﻿using TyzenR.Account.Entity;

namespace TyzenR.Taskman.Managers
{
    public interface IAppInfo
    {
        Guid CurrentUserId { get; }

        DateTime GetCurrentDateTime();

        UserEntity GetCurrentUser();

        string CurrentTimeZoneId { get; }

        DateTime ConvertToUtc(DateTime date);
    }
}