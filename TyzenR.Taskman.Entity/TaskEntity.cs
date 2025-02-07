using TyzenR.EntityLibrary;

namespace TyzenR.Taskman.Entity
{
    public class TaskEntity : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public int Points { get; set; } 
        public double Hours { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedBy { get; set; }
        public Guid AssignedTo { get; set; }

        public DateTime UpdatedOn { get; set; }
        public Guid UpdatedBy { get; set; }
        public string UpdatedIP { get; set; }
    }
}
