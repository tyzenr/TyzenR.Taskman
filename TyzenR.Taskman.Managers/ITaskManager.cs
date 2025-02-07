using TyzenR.Account.Entity;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public interface ITaskManager
    {
        Task<IList<TaskEntity>> GetTasksByUserAsync(UserEntity user);
    }
}
