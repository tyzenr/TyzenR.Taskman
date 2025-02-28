namespace TyzenR.Taskman.Entity
{
    public class ActionModel
    {
        public ActionTypeEnum Type { get; set; }
        public Guid UserId { get; set; }    
        public string UserName { get; set; }

        public string UpdatedIpAddress { get; set; } = string.Empty;
        public DateTime UpdatedOn = DateTime.UtcNow;
        public string EntityJson { get; set; } = string.Empty;    
    }
}
