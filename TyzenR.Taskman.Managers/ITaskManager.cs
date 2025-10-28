using TyzenR.Account.Entity;
using TyzenR.EntityLibrary;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public interface ITaskManager : IRepository<TaskEntity>
    {
        Task<IList<TaskEntity>> GetPaginatedTasksForUserAsync(IQueryable<TaskEntity> query, int page, int pageSize, string sortBy, SortDirectionEnum sortDirection);
        Task<IList<TaskEntity>> GetTasksForUserAsync(UserEntity user);
        Task<IList<UserEntity>> GetManagersAsync(UserEntity user);
        Task<IList<TeamMemberModel>> GetTeamMembersAsync(UserEntity user, string firstItem = "");
        Task NotifyManagersAsync(UserEntity user, TaskEntity task, string title);
        Task NotifyUserAsync(TaskEntity task, string title);

        Task<IList<TaskEntity>> GetTimesheetsAsync(Guid userId, DateTime fromDate, DateTime toDate);
        string GetTotalTimeFormatted(IList<TaskEntity> list);
    }
}
