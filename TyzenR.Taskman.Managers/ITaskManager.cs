using TyzenR.Account.Entity;
using TyzenR.EntityLibrary;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public interface ITaskManager : IRepository<TaskEntity>
    {
        Task<IList<TaskEntity>> GetTasksByUserAsync(UserEntity user);
        Task<bool> IsValidAsync(TaskEntity task);
        Task<(bool, TaskEntity)> CreateTaskAsync(TaskEntity task);
    }
}
