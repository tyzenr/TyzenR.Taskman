using System.ComponentModel.DataAnnotations.Schema;
using TyzenR.EntityLibrary;

namespace TyzenR.Taskman.Entity
{
    [Table("TaskAttachment")]
    public class TaskAttachmentEntity : BaseEntity
    {
        public Guid TaskId { get; set; }

        public string FileName { get; set; }

        public byte[] FileContent { get; set; }

        public string BlobUri { get; set; }
    }
}
