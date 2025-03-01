using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Radzen;
using System;
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

        public async Task<bool> TrackActionAsync(ActionTypeEnum actionType, TaskEntity entity)
        {
            try
            {
                var actionTracker = await entityContext.ActionTrackers
                    .Where(t => t.EntityId == entity.Id)
                    .FirstOrDefaultAsync();

                if (actionTracker == null)
                {
                    actionTracker = new ActionTrackerEntity()
                    {
                        EntityId = entity.Id,
                    };
                    await entityContext.ActionTrackers.AddAsync(actionTracker);
                    await entityContext.SaveChangesAsync();
                }

                if (actionTracker.Actions == null)
                {
                    actionTracker.Actions = new List<ActionModel>();
                }

                actionTracker.Actions.Add(new ActionModel()
                {
                    Type = actionType,
                    UpdatedOn = appInfo.GetCurrentDateTime(),
                    UserId = appInfo.CurrentUserId,
                    UpdatedIpAddress = appInfo.CurrentUserIPAddress,
                    EntityJson = JsonConvert.SerializeObject(entity)
                });

                var success = await this.UpdateAsync(actionTracker);
                base.DetachAll();

                return success;
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
