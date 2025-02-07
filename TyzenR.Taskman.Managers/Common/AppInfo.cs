using TyzenR.Account.Entity;
using TyzenR.Account.Managers;
using TyzenR.Investor.Managers.Extensions.EmailService;

namespace TyzenR.Taskman.Managers
{
    public class AppInfo : IAppInfo
    {
        private readonly IUserManager userManager;
        private readonly IEmailConfig emailConfig;

        public AppInfo(
            IUserManager userManager,
            IEmailConfig emailConfig)
        {
            this.userManager = userManager ?? throw new ApplicationException("User Manager is null!");
            this.emailConfig = emailConfig ?? throw new ApplicationException("Email Config is null!");
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

            return null;
        }

        public DateTime ConvertToUtc(DateTime date)
        {
            var result = TimeZoneInfo.ConvertTimeToUtc(date, TimeZoneInfo.FindSystemTimeZoneById(this.CurrentTimeZoneId));

            return result;
        }
    }
}