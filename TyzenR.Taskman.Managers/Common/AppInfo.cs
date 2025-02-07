using System.Diagnostics;
using TyzenR.Account.Entity;
using TyzenR.Account.Managers;
namespace TyzenR.Taskman.Managers
{
    public class AppInfo : IAppInfo
    {
        private readonly IUserManager userManager;

        public AppInfo(
            IUserManager userManager)
        {
            this.userManager = userManager ?? throw new ApplicationException("Instance is null!");
        }

        public DateTime GetCurrentDateTime()
        {
            return DateTime.UtcNow;
        }

        public string CurrentTimeZoneId
        {
            get { return "India Standard Time"; } // TODO: Add TimeZone to settings ON multinational expansion
        }

        public Guid CurrentUserId
        {
            get
            {
                var user = userManager.CurrentUser;

                if (user != null)
                {
                    return user.Id;
                }

                return Guid.Empty;
            }
        }

        public UserEntity GetCurrentUser()
        {
            var user = userManager.CurrentUser;

            if (user != null)
            {
                return new UserEntity()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                };
            }

            if (Debugger.IsAttached)
            {
                return new UserEntity()
                {
                    Id = Guid.Empty,
                    FirstName = "Dev",
                };
            }

            return null;
        }

        public DateTime ConvertToUtc(DateTime date)
        {
            var result = TimeZoneInfo.ConvertTimeToUtc(date, TimeZoneInfo.FindSystemTimeZoneById(this.CurrentTimeZoneId));

            return result;
        }
    }
}