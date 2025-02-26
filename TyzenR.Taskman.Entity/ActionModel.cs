namespace TyzenR.Taskman.Entity
{
    public class ActionModel
    {
        public ActionTypeEnum ActionType { get; set; }
        public Guid ActionUserId { get; set; }    
        public DateTime ActionOn = DateTime.UtcNow;
        public string TaskJson { get; set; } = string.Empty;    
    }
}
