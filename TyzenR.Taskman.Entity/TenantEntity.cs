using System.ComponentModel.DataAnnotations.Schema;
using TyzenR.EntityLibrary;

namespace TyzenR.Taskman.Entity
{
    [Table("Tenant")]
    public class TenantEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Title { get; set; }   
        public string Email { get; set; }   
        public string MobileNumber { get; set; }
        public string PAN { get; set; }
        public string AadharNumber { get; set; }
        public string Notes { get; set; }
        public string NotesToTenant { get; set; }

        public string RentReminderEmails { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}


