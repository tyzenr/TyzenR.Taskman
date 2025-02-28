using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Radzen;
using TyzenR.Account.Managers;
using TyzenR.EntityLibrary;
using TyzenR.Publisher.Shared;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public class ActionTrackerManager : BaseRepository<ActionTrackerEntity>, IActionTrackerManager
    {
        private readonly EntityContext entityContext;
        private readonly IUserManager userManager;
        private readonly IAppInfo appInfo;

        public ActionTrackerManager(
            EntityContext entityContext,
            IUserManager userManager,
            IAppInfo appInfo) : base(entityContext)
        {
            this.entityContext = entityContext ?? throw new ApplicationException("Instance is null!");
            this.userManager = userManager ?? throw new ApplicationException("Instance is null!");
            this.appInfo = appInfo ?? throw new ApplicationException("Instance is null!");
        }

        public async Task<bool> SaveActionAsync(ActionTypeEnum actionType, TaskEntity entity)
        {
            try
            {
                var actionTracker = entityContext.ActionTrackers.Where(t => t.EntityId == entity.Id).FirstOrDefault();

                if (actionTracker == null)
                {
                    actionTracker = new ActionTrackerEntity()
                    {
                        EntityId = entity.Id,
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
                    entityContext.ActionTrackers.Add(actionTracker);
                }
                else
                {
                    if (actionTracker.Actions == null)
                    {
                        actionTracker.Actions = new List<ActionModel>();
                    }

                    actionTracker.Actions.Add(new ActionModel()
                    {
                        Type = actionType,
                        UpdatedOn = appInfo.GetCurrentDateTime(),
                        UserId = appInfo.CurrentUserId,
                        EntityJson = JsonConvert.SerializeObject(entity)
                    });
                }

                entityContext.Entry(actionTracker).State = EntityState.Modified;
                await entityContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                await SharedUtility.SendEmailToModeratorAsync("Taskman.TaskManager.ChangeTrackAsync.Exception", ex.ToString());
                return false;
            }
        }

        public async Task<IList<ActionModel>> GetActionsAsync(Guid entityId)
        {
            var result = new List<ActionModel>();

            try
            {
                var actionTracker = entityContext.ActionTrackers.Where(t => t.EntityId == entityId).FirstOrDefault();
                if (actionTracker != null)
                {
                    result = actionTracker.Actions.OrderBy(a => a.UpdatedOn).ToList();

                    foreach (var action in result)
                    {
                        var user = await userManager.GetByIdAsync(action.UserId);
                        action.UserName = user?.FirstName;
                    }
                }
            }
            catch (Exception ex)
            {
                await SharedUtility.SendEmailToModeratorAsync("Taskman.TaskManager.GetActionsAsync.Exception", ex.ToString());
            }

            return result;
        }
    }
}
