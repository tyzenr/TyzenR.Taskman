using TyzenR.EntityLibrary;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public interface IAttachmentManager : IRepository<AttachmentEntity>
    {
        Task<IList<AttachmentEntity>> GetAllByParentIdAsync(Guid parentId);
        Task<bool> SaveAttachmentsAsync(IList<AttachmentEntity> attachments, Guid parentId, IList<AttachmentEntity> deleted = null);
        Task<List<string>> GetStringAttachmentsAsync(IList<AttachmentEntity> emailAttachments);
    }
}
