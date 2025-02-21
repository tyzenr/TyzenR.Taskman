using System.ComponentModel.DataAnnotations.Schema;
using TyzenR.EntityLibrary;

namespace TyzenR.Taskman.Entity
{
    [Table("EmailTracker")]
    public class EmailTrackerEntity : BaseEntity
    {
        public Guid EmailId { get; set; } = Guid.Empty;
        public string Subject { get; set; } = string.Empty;
        public string ToEmail { get; set; }
        public DateTime SentOn { get; set; } = DateTime.UtcNow;
    }
}