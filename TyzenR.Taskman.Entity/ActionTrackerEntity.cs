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

        public string ActionsJson
        {
            get
            {
                var result = JsonConvert.SerializeObject(Actions);

                if (string.IsNullOrWhiteSpace(result))
                {
                    return string.Empty;
                }

                return result;
            }
            set
            {
                var actionsJson = value;

                try
                {
                    Actions = JsonConvert.DeserializeObject<IList<ActionModel>>(actionsJson);
                }
                catch (Exception ex)
                {
                    Actions = new List<ActionModel>();
                }
            }
        }

        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
    }
}
