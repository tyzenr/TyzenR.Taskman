﻿using TyzenR.EntityLibrary;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public interface IActionTrackerManager : IRepository<ActionTrackerEntity>
    {
        Task<bool> TrackActionAsync(ActionTypeEnum actionType, TaskEntity entity);
        Task<IList<ActionModel>> GetActionsAsync(Guid entityId);
    }
}
