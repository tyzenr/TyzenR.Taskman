using System.ComponentModel.DataAnnotations.Schema;
using TyzenR.EntityLibrary;

namespace TyzenR.Taskman.Entity
{
    [Table("TaskTracker")]
    public class TaskTrackerEntity : BaseEntity
    {
        public Guid TaskId { get; set; }
        public string ActionsJson { get; set; } = string.Empty;

        [NotMapped]
        public IList<ActionModel> Actions { get; set; } = new List<ActionModel>();

        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
    }
}
