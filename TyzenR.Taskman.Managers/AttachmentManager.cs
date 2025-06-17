using TyzenR.Account.Managers;
using TyzenR.Account;
using TyzenR.EntityLibrary;
using TyzenR.Taskman.Entity;
using Microsoft.EntityFrameworkCore;

namespace TyzenR.Taskman.Managers
{
    public class AttachmentManager : BaseRepository<AttachmentEntity>, IAttachmentManager
    {
        private readonly EntityContext entityContext;
        private readonly AccountContext accountContext;
        private readonly IUserManager userManager;
        private readonly IActionTrackerManager actionManager;
        private readonly IAppInfo appInfo;

        public AttachmentManager(
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

        public async Task<IList<AttachmentEntity>> GetAllByParentIdAsync(Guid parentId)
        {
            var result = await this.entityContext.Attachments.Where(a => a.ParentId == parentId)
                .ToListAsync();

            return result;
        }

        public async Task<bool> SaveAsync(AttachmentEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
