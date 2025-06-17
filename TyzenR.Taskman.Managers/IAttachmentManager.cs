using TyzenR.EntityLibrary;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public interface IAttachmentManager : IRepository<AttachmentEntity>
    {
        Task<bool> SaveAsync(AttachmentEntity entity);

        Task<IList<AttachmentEntity>> GetAllByParentIdAsync(Guid parentId);
    }
}
