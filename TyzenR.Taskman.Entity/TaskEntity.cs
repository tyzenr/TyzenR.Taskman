using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using TyzenR.EntityLibrary;
using TyzenR.Taskman.Entity.Models;

namespace TyzenR.Taskman.Entity
{
    [Table("Task")]
    public class TaskEntity : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime? Date { get; set; } = DateTime.UtcNow;
        public string Notes { get; set; } = string.Empty;

        public TaskStatusEnum Status { get; set; }
        public TaskTypeEnum Type { get; set; } = TaskTypeEnum.Task;

        public double Points { get; set; } = 1;
        public double Hours { get; set; } = 8;

        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;  
        public Guid AssignedTo { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
        public Guid UpdatedBy { get; set; }

        public string UpdatedIP { get; set; } = string.Empty;


        [NotMapped]
        public string AssignedToName { get; set; } = string.Empty;

        public List<TimesheetTaskModel> GetTimesheetItems()
        {
            return JsonConvert.DeserializeObject<List<TimesheetTaskModel>>(this.Description);
        }

        public string GetTotalTime()
        {
            int totalHours = 0, totalMinutes = 0;
            foreach (var item in GetTimesheetItems())
            {
                totalHours = totalHours + item.Hours;
                totalMinutes = totalMinutes + item.Minutes; 
            }

            totalHours = totalHours + (totalMinutes / 60);
            totalMinutes = totalMinutes % 60;

            return $"{totalHours}h {totalMinutes}m";
        }
    }
}
