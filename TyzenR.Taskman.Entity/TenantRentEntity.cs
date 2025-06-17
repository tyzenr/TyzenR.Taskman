using System.ComponentModel.DataAnnotations.Schema;
using TyzenR.EntityLibrary;

namespace TyzenR.Taskman.Entity
{
    [Table("TenantRent")]
    public class TenantRentEntity : BaseEntity
    {
        public int Month { get; set; }  
        public int Year { get; set; }   
        public double RentAmount { get; set; }
        public double PaidAmount { get; set; }
        public bool IsComplete { get; set; }   
        public string Notes { get; set; }
        public string NotesToTenant { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}


