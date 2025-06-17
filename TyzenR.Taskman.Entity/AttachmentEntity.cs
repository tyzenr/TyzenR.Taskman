using System.ComponentModel.DataAnnotations.Schema;
using TyzenR.EntityLibrary;

namespace TyzenR.Taskman.Entity
{
    [Table("Attachment")]
    public class AttachmentEntity : BaseEntity
    {
        public Guid ParentId { get; set; }

        public string FileName { get; set; }

        public byte[] FileContent { get; set; }

        public string BlobUri { get; set; }
    }
}
