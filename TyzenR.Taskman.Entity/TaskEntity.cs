using System.ComponentModel.DataAnnotations.Schema;
using TyzenR.EntityLibrary;

namespace TyzenR.Taskman.Entity
{
    [Table("Task")]
    public class TaskEntity : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public TaskStatusEnum Status { get; set; }

        public double Points { get; set; } = 1;
        public double Hours { get; set; } = 8;

        public Guid CreatedBy { get; set; }
        public Guid AssignedTo { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
        public Guid UpdatedBy { get; set; }

        public string UpdatedIP { get; set; } = string.Empty;

        public IList<TaskAttachmentEntity> Attachments { get; set; } = new List<TaskAttachmentEntity>();    
    }
}
