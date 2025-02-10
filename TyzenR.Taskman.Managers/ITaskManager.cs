using TyzenR.Account.Entity;
using TyzenR.EntityLibrary;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public interface ITaskManager : IRepository<TaskEntity>
    {
        Task<IList<TaskEntity>> GetTasksForUserAsync(UserEntity user);
        Task<IList<UserEntity>> GetManagersAsync(UserEntity user);
        Task<IList<TeamMemberEntity>> GetTeamMembersAsync(UserEntity user);
    }
}
