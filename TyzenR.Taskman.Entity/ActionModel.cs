namespace TyzenR.Taskman.Entity
{
    public class ActionModel
    {
        public ActionTypeEnum Type { get; set; }
        public Guid UserId { get; set; }    
        public DateTime UpdatedOn = DateTime.UtcNow;
        public string UpdatedId { get; set; } = string.Empty;   
        public string EntityJson { get; set; } = string.Empty;    
    }
}
