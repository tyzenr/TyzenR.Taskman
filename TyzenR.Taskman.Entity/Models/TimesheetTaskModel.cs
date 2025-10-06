namespace TyzenR.Taskman.Entity.Models
{
    public class TimesheetTaskModel
    {
        public string Description { get; set; } = string.Empty; 
        public int Hours { get; set; }
        public int Minutes { get; set; }
    }
}
