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
        private readonly IAppInfo appInfo;

        public TaskManager(
            EntityContext entityContext,
            AccountContext accountContext,
            IUserManager userManager,
            IActionTrackerManager actionManager,
            IAppInfo appInfo) : base(entityContext)
        {
            this.entityContext = entityContext ?? throw new ApplicationException("Instance is null!");
            this.accountContext = accountContext ?? throw new ApplicationException("Instance is null!");
            this.userManager = userManager ?? throw new ApplicationException("Instance is null!");
            this.actionManager = actionManager ?? throw new ApplicationException("Instance is null!");
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

        public async Task<IList<TaskEntity>> GetPaginatedTasksForUserAsync(IQueryable<TaskEntity> query, int page, int pageSize, string sortBy, SortDirection direction)
        {
            switch (sortBy)
            {
                case "Title":
                    query = direction == SortDirection.Ascending
                        ? query.OrderBy(e => e.Title)
                        : query.OrderByDescending(e => e.Title);
                    break;

                case "Description":
                    query = direction == SortDirection.Ascending
                        ? query.OrderBy(e => e.Description)
                        : query.OrderByDescending(e => e.Description);
                    break;

                case "Status":
                    query = direction == SortDirection.Ascending
                        ? query.OrderBy(e => e.Status)
                        : query.OrderByDescending(e => e.Description);
                    break;

                default:
                    query = query.OrderBy(t => t.Status)
                        .ThenByDescending(t => t.UpdatedOn);
                    break;
            }

            return await query.Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync();
        }

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
                    body = $"User: {user.FirstName}".Break() +
                        $"Title: {task.Title}".Break() +
                        $"Description: {task.Description}".Break() +
                        $"Status: {task.Status.ToString()}".Break() +
                        $"UpdatedOn: {task.UpdatedOn}".Break() +
                        $"Url: {Constants.ApplicationUrl}/task/edit/{task.Id}".Break();

                    if (manager.Email == "contact@futurecaps.com")
                    {
                        body += $"UpdatedIP: {task.UpdatedIP}".Break();
                    }

                    await appInfo.SendEmailAsync(manager.Email, title, body);
                }
            }
            catch (Exception ex)
            {
                await SharedUtility.SendEmailToModeratorAsync("Taskman.TaskManager.NotifyManagersAsync.Exception", "ip: " + appInfo.CurrentUserIPAddress + "  " + ex.ToString().Break() + body);
            }
        }

        public async Task NotifyUserAsync(TaskEntity task, string title)
        {
            try
            {
                if (task.AssignedTo != appInfo.CurrentUserId)
                {
                    var user = await userManager.GetByIdAsync(task.AssignedTo);
                    await appInfo.SendEmailAsync(user.Email, title, $"Visit: {Constants.ApplicationUrl} for details");
                }
            }
            catch (Exception ex)
            {
                await SharedUtility.SendEmailToModeratorAsync("Taskman.TaskManager.NotifyUserAsync.Exception", "ip: " + appInfo.CurrentUserIPAddress + "  " + ex.ToString().Break());
            }
        }
    }
}
