using TyzenR.Account.Entity;
using TyzenR.EntityLibrary;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public interface ITaskManager : IRepository<TaskEntity>
    {
        Task<IList<TaskEntity>> GetPaginatedTasksForUserAsync(IQueryable<TaskEntity> query, int page, int pageSize, string sortBy, SortDirection sortDirection);
        Task<IList<TaskEntity>> GetTasksForUserAsync(UserEntity user);
        Task<IList<UserEntity>> GetManagersAsync(UserEntity user);
        Task<IList<MemberModel>> GetTeamMembersAsync(UserEntity user);
        Task NotifyManagersAsync(UserEntity user, TaskEntity task, string title);
        Task NotifyUserAsync(TaskEntity task, string title);
        Task<string> UploadAttachmentToBlobAsync(Stream fileStream, string originalFileName, string contentType);
        Task<List<AttachmentEntity>> GetAttachmentsAsync(TaskEntity task);
    }
}
