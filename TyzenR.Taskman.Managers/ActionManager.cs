using Newtonsoft.Json;
using TyzenR.EntityLibrary;
using TyzenR.Publisher.Shared;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public class ActionManager : BaseRepository<ActionEntity>, IActionManager
    {
        private readonly EntityContext entityContext;
        private readonly IAppInfo appInfo;

        public ActionManager(
            EntityContext entityContext,
            IAppInfo appInfo) : base(entityContext)
        {
            this.entityContext = entityContext ?? throw new ApplicationException("Instance is null!");
            this.appInfo = appInfo ?? throw new ApplicationException("Instance is null!");
        }

        public async Task<bool> SaveActionAsync(ActionTypeEnum actionType, TaskEntity entity)
        {
            try
            {
                var action = entityContext.Actions.Where(t => t.EntityId == entity.Id).FirstOrDefault();

                if (action == null)
                {
                    action = new ActionEntity()
                    {
                        Actions = new List<ActionModel>()
                        {
                            new ActionModel()
                            {
                                Type = actionType,
                                UpdatedOn = appInfo.GetCurrentDateTime(),
                                UserId = appInfo.CurrentUserId,
                                EntityJson = JsonConvert.SerializeObject(entity)
                            }
                        }
                    };
                    entityContext.Actions.Add(action);
                }
                else
                {
                    if (action.Actions == null)
                    {
                        action.Actions = new List<ActionModel>();
                    }


                    action.Actions.Add(new ActionModel()
                    {
                        Type = actionType,
                        UpdatedOn = appInfo.GetCurrentDateTime(),
                        UserId = appInfo.CurrentUserId,
                        EntityJson = JsonConvert.SerializeObject(entity)
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                await SharedUtility.SendEmailToModeratorAsync("Taskman.TaskManager.ChangeTrackAsync.Exception", ex.ToString());
                return false;
            }
        }

        public Task<IList<ActionModel>> GetActionsAsync(Guid entityId)
        {
            throw new NotImplementedException();
        }
    }
}
