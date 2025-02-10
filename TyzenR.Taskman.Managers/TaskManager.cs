using Microsoft.EntityFrameworkCore;
using TyzenR.Account;
using TyzenR.Account.Entity;
using TyzenR.EntityLibrary;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public class TaskManager : BaseRepository<TaskEntity>, ITaskManager
    {
        private readonly EntityContext context;
        private readonly AccountContext accountContext;

        public TaskManager(EntityContext context, AccountContext accountContext) : base(context)
        {
            this.context = context ?? throw new ApplicationException("Instance is null!");
            this.accountContext = accountContext ?? throw new ApplicationException("Instance is null!");
        }

        public async Task<IList<UserEntity>> GetManagersAsync(UserEntity user)
        {
            if (user == null)
            {
                return new List<UserEntity>();      
            }

            var managerIds = await this.context.Teams
                .Where(t => t.MemberId == user.Id)
                .Select(t => t.ManagerId)
                .ToListAsync();

            var users = await this.accountContext.Users
                .Where(u => managerIds.Contains(u.Id))
                .ToListAsync();

            return users;
        }

        public async Task<IList<TaskEntity>> GetTasksForUserAsync(UserEntity user)
        {
            if (user == null)
            {
                return new List<TaskEntity>();
            }

            var result = await this.context.Tasks
                .Where(t => (t.Status == TaskStatusEnum.InProgress || t.Status == TaskStatusEnum.Completed) && (t.CreatedBy == user.Id || t.AssignedTo == user.Id))
                .OrderBy(t => t.Status)
                .ThenByDescending(t => t.UpdatedOn)   
                .ToListAsync();

            return result;
        }

        public async Task<IList<TeamMemberEntity>> GetTeamMembersAsync(UserEntity user)
        {
            if (user == null)
            {
                return new List<TeamMemberEntity>();
            }

            var members = await this.context.Teams
                .Where(t => t.ManagerId == user.Id)
                .ToListAsync();

            var users = await this.accountContext.Users
                .Where(u => members.Select(m => m.MemberId).Contains(u.Id))
                .ToListAsync();

            var result = new List<TeamMemberEntity>();
            result.Add(new TeamMemberEntity() { Id = user.Id, Name = user.FirstName });     
            foreach(var member in members)
            {
                result.Add(new TeamMemberEntity() { Id = member.Id, Name = users.FirstOrDefault(u => u.Id == member.Id).FirstName });
            }

            return result;
        }
    }
}
