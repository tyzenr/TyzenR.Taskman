using System.ComponentModel.DataAnnotations.Schema;
using TyzenR.EntityLibrary;

namespace TyzenR.Taskman.Entity
{
    [Table("Team")]
    public class TeamEntity : BaseEntity
    {
        public Guid MemberId { get; set; }
        public Guid ManagerId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public string Name { get; set; } = string.Empty;

        [NotMapped]
        public string Email { get; set; } = string.Empty;
    }
}
