using Microsoft.EntityFrameworkCore;
using TyzenR.Account;
using TyzenR.Account.Entity;
using TyzenR.Account.Managers;
using TyzenR.EntityLibrary;
using TyzenR.Publisher.Shared;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public class TaskManager : BaseRepository<TaskEntity>, ITaskManager
    {
        private readonly EntityContext entityContext;
        private readonly AccountContext accountContext;
        private readonly IUserManager userManager;
        private readonly IActionTrackerManager actionManager;
        private readonly IAttachmentManager attachmentManager;
        private readonly IAppInfo appInfo;

        public TaskManager(
            EntityContext entityContext,
            AccountContext accountContext,
            IUserManager userManager,
            IActionTrackerManager actionManager,
            IAttachmentManager attachmentManager,
            IAppInfo appInfo) : base(entityContext)
        {
            this.entityContext = entityContext ?? throw new ApplicationException("Instance is null!");
            this.accountContext = accountContext ?? throw new ApplicationException("Instance is null!");
            this.userManager = userManager ?? throw new ApplicationException("Instance is null!");
            this.actionManager = actionManager ?? throw new ApplicationException("Instance is null!");
            this.attachmentManager = attachmentManager ?? throw new ApplicationException("Instance is null!");
            this.appInfo = appInfo ?? throw new ApplicationException("Instance is null!");
        }

        public async Task<IList<UserEntity>> GetManagersAsync(UserEntity user)
        {
            if (user == null)
            {
                return new List<UserEntity>();
            }

            var managerIds = await this.entityContext.Teams
                .Where(t => t.MemberId == user.Id)
                .Select(t => t.ManagerId)
                .ToListAsync();

            var users = await this.accountContext.Users
                .Where(u => managerIds.Contains(u.Id))
                .ToListAsync();

            return users;
        }

        public async Task<IList<TaskEntity>> GetPaginatedTasksForUserAsync(IQueryable<TaskEntity> query, int page, int pageSize, string sortBy, SortDirectionEnum direction)
        {
            switch (sortBy)
            {
                case "Title":
                    query = direction == SortDirectionEnum.Ascending
                        ? query.OrderBy(e => e.Title)
                        : query.OrderByDescending(e => e.Title);
                    break;

                case "Description":
                    query = direction == SortDirectionEnum.Ascending
                        ? query.OrderBy(e => e.Description)
                        : query.OrderByDescending(e => e.Description);
                    break;

                case "Status":
                    query = direction == SortDirectionEnum.Ascending
                        ? query.OrderBy(e => e.Status)
                        : query.OrderByDescending(e => e.Description);
                    break;

                case "AssignedTo":
                    query = direction == SortDirectionEnum.Ascending
                        ? query.OrderBy(e => e.AssignedTo)
                        : query.OrderByDescending(e => e.Description);
                    break;

                case "CreatedOn":
                    query = direction == SortDirectionEnum.Ascending
                        ? query.OrderBy(e => e.CreatedOn)
                        : query.OrderByDescending(e => e.CreatedOn);
                    break;

                default:
                    query = query.OrderBy(t => t.Status)
                        .ThenByDescending(t => t.UpdatedOn);
                    break;
            }

            var result = await query.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Populate AssignedToName for each task    
            foreach (var task in result)
            {
                if (idNameDictionary.ContainsKey(task.AssignedTo))
                {
                    task.AssignedToName = idNameDictionary[task.AssignedTo];
                }
                else
                {
                    var user = userManager.GetUserById(task.AssignedTo);
                    if (user != null)
                    {
                        task.AssignedToName = user.FirstName;
                        idNameDictionary.Add(task.AssignedTo, task.AssignedToName);
                    }
                }
            }

            return result;
        }

        private IDictionary<Guid, string> idNameDictionary = new Dictionary<Guid, string>();

        public async Task<IList<TaskEntity>> GetTasksForUserAsync(UserEntity user)
        {
            if (user == null)
            {
                return new List<TaskEntity>();
            }

            var result = await this.entityContext.Tasks
                .Where(t => (t.Status == TaskStatusEnum.InProgress || t.Status == TaskStatusEnum.Completed) && (t.CreatedBy == user.Id || t.AssignedTo == user.Id))
                .OrderBy(t => t.Status)
                .ThenByDescending(t => t.UpdatedOn)
                .ToListAsync();

            return result;
        }

        public async Task<IList<MemberModel>> GetTeamMembersAsync(UserEntity user)
        {
            if (user == null)
            {
                return new List<MemberModel>();
            }

            var members = await this.entityContext.Teams
                .Where(t => t.ManagerId == user.Id)
                .ToListAsync();

            var users = await this.accountContext.Users
                .Where(u => members.Select(m => m.MemberId).Contains(u.Id))
                .ToListAsync();

            var result = new List<MemberModel>();
            result.Add(new MemberModel() { Id = user.Id, Name = user.FirstName });
            foreach (var member in members)
            {
                if (!result.Any(r => r.Id == member.MemberId))
                {
                    result.Add(new MemberModel() { Id = member.MemberId, Name = users.FirstOrDefault(u => u.Id == member.MemberId)?.FirstName });
                }
            }

            return result;
        }

        public async Task NotifyManagersAsync(UserEntity user, TaskEntity task, string title)
        {
            string body = string.Empty;

            try
            {
                var managers = await GetManagersAsync(user);

                foreach (var manager in managers)
                {
                    body = "User:".Bold() + $" {user.FirstName}".Break() +
                        "Title: ".Bold() + $"{task.Title}".Break() +
                        "Description: ".Bold() + $"{task.Description.AddBreaks()}".Break() +
                        "Status: ".Bold() + $"{task.Status.ToString()}".Break() +
                        "UpdatedOn: ".Bold() + $"{task.UpdatedOn}".Break() +
                        "Url: ".Bold() + GetUrl(task) .Break();

                    if (manager.Email == "contact@futurecaps.com")
                    {
                        body += $"UpdatedIP: {task.UpdatedIP}".Break();
                    }

                    var attachments = await attachmentManager.GetAllByParentIdAsync(task.Id);
                    List<string> stringAttachments = await attachmentManager.GetStringAttachmentsAsync(attachments);

                    await appInfo.SendEmailAsync(manager.Email, title, body, false, stringAttachments);
                }
            }
            catch (Exception ex)
            {
                await SharedUtility.SendEmailToModeratorAsync("Taskman.TaskManager.NotifyManagersAsync.Exception", "ip: " + appInfo.CurrentUserIPAddress + "  " + ex.ToString().Break() + body);
            }
        }

        private string GetUrl(TaskEntity task)
        {
            if (task.Type == TaskTypeEnum.Normal)
            {
                return $"{TaskmanConstants.ApplicationUrl}/task/edit/{task.Id}";
            }
            else
            {
                return $"{TaskmanConstants.ApplicationUrl}/task/timesheet/edit/{task.Id}";
            }

            throw new NotImplementedException();
        }

        public async Task NotifyUserAsync(TaskEntity task, string title)
        {
            try
            {
                if (task.AssignedTo != appInfo.CurrentUserId)
                {
                    var user = await userManager.GetByIdAsync(task.AssignedTo);
                    await appInfo.SendEmailAsync(user.Email, title, $"Visit: {TaskmanConstants.ApplicationUrl} for details");
                }
            }
            catch (Exception ex)
            {
                await SharedUtility.SendEmailToModeratorAsync("Taskman.TaskManager.NotifyUserAsync.Exception", "ip: " + appInfo.CurrentUserIPAddress + "  " + ex.ToString().Break());
            }
        }
    }
}
