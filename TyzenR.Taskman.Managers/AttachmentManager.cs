using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using TyzenR.Account;
using TyzenR.Account.Managers;
using TyzenR.EntityLibrary;
using TyzenR.Publisher.Shared;
using TyzenR.Publisher.Shared.Constants;
using TyzenR.Taskman.Entity;

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

        public async Task<bool> SaveAttachmentsAsync(IList<AttachmentEntity> attachments, Guid parentId, IList<AttachmentEntity> deleted = null)
        {
            try
            {
                // Save
                foreach (var attachment in attachments)
                {
                    // Save to Blob
                    if (attachment.FileContent != null && !string.IsNullOrEmpty(attachment.FileName) && string.IsNullOrEmpty(attachment.BlobUri))
                    {
                        using var stream = new MemoryStream(attachment.FileContent);

                        string contentType = FileExtensions.GetMimeTypeFromUrl(attachment.FileName);
                        attachment.BlobUri = await UploadAttachmentToBlobAsync(stream, attachment.FileName, contentType);
                    }

                    // Save to Db
                    if (attachment.ParentId == Guid.Empty)
                    {
                        attachment.ParentId = parentId;
                        if (attachment.Id == Guid.Empty)
                        {
                            await InsertAsync(attachment);
                        }
                    }
                }

                // Delete
                if (deleted != null)
                {
                    foreach (var attachment in deleted)
                    {
                        if (attachment.Id != Guid.Empty)
                        {
                            // Delete from Blob
                            if (!string.IsNullOrEmpty(attachment.BlobUri))
                            {
                                BlobServiceClient blobServiceClient = new BlobServiceClient(PublisherConstants.StorageConnectionString);
                                var blobContainerClient = blobServiceClient.GetBlobContainerClient(PublisherConstants.BlobContainerName);
                                var blobClient = blobContainerClient.GetBlobClient(Path.GetFileName(attachment.BlobUri));

                                await blobClient.DeleteIfExistsAsync();
                            }

                            // Delete from Db
                            await DeleteAsync(attachment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await SharedUtility.SendEmailToModeratorAsync("Taskman.AttachmentManager.SaveAttachmentsAsync", ex.ToString());

                return false;
            }

            return true;
        }

        private async Task<string> UploadAttachmentToBlobAsync(Stream fileStream, string originalFileName, string contentType)
        {
            if (fileStream == null || string.IsNullOrWhiteSpace(originalFileName))
            {
                throw new ArgumentException("Invalid file or file name.");
            }

            var restrictedExtensions = new[] { ".exe" };
            var fileExtension = Path.GetExtension(originalFileName).ToLower();

            if (restrictedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException($"File type '{fileExtension}' is restricted.");
            }

            BlobServiceClient blobServiceClient = new BlobServiceClient(PublisherConstants.StorageConnectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(PublisherConstants.BlobContainerName);

            string blobName = $"{Guid.NewGuid()}_{originalFileName}";
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(fileStream, overwrite: true);

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };
            await blobClient.SetHttpHeadersAsync(blobHttpHeaders);

            return blobClient.Uri.ToString();
        }
    }
}
