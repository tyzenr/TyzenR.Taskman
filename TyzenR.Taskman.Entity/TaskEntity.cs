using System.ComponentModel.DataAnnotations.Schema;
using TyzenR.EntityLibrary;

namespace TyzenR.Taskman.Entity
{
    [Table("Team")]
    public class TeamEntity : BaseEntity
    {
        public Guid MemberId { get; set; }
        public Guid ManagerId { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
    }
}
