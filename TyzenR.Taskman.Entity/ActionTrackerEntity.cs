using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using TyzenR.EntityLibrary;

namespace TyzenR.Taskman.Entity
{
    [Table("ActionTracker")]
    public class ActionTrackerEntity : BaseEntity
    {
        public Guid EntityId { get; set; }
        
        [NotMapped]
        public IList<ActionModel> Actions { get; set; } = new List<ActionModel>();

        private string actionsJson;
        public string ActionsJson
        {
            get
            {
                return JsonConvert.SerializeObject(Actions);
            }
            set
            {
                actionsJson = value;
                Actions = JsonConvert.DeserializeObject<IList<ActionModel>>(actionsJson);
            }
        }

        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
    }
}
