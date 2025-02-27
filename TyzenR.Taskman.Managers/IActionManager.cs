using TyzenR.EntityLibrary;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public interface IActionManager : IRepository<ActionEntity>
    {
        Task<bool> SaveActionAsync(ActionTypeEnum actionType, TaskEntity entity);
        Task<IList<ActionModel>> GetActionsAsync(Guid entityId);
    }
}
