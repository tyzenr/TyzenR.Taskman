using System.ComponentModel.DataAnnotations.Schema;
using TyzenR.EntityLibrary;

namespace TyzenR.Taskman.Entity
{
    [Table("Task")]
    public class TaskEntity : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public TaskStatusEnum Status { get; set; }

        public int Points { get; set; } 
        public double Hours { get; set; }

        public Guid CreatedBy { get; set; }
        public Guid AssignedTo { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
        public Guid UpdatedBy { get; set; }
        public string UpdatedIP { get; set; }
    }
}
