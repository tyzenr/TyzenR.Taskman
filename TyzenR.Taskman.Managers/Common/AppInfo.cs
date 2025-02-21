using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using TyzenR.Account.Entity;
using TyzenR.Account.Managers;
using TyzenR.Publisher.Shared;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public class AppInfo : IAppInfo
    {
        private readonly IUserManager userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly EntityContext entityContext;

        public AppInfo(IUserManager userManager,
            IHttpContextAccessor httpContextAccessor,
            EntityContext entityContext)
        {
            this.userManager = userManager ?? throw new ApplicationException("Instance is null!");
            this.httpContextAccessor = httpContextAccessor ?? throw new ApplicationException("Instance is null!");
            this.entityContext = entityContext ?? throw new ApplicationException("Instance is null!");
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

        public string CurrentUserIPAddress
        {
            get { return httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown"; }
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

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool tracking = true)
        {
            await SharedUtility.SendEmailAsync(toEmail, subject, body, "contact@futurecaps.com", Constants.ApplicationTitle);

            if (tracking)
            {
                await TrackEmailAsync(Guid.Empty, subject, toEmail);
            }
        }

        public async Task TrackEmailAsync(Guid emailId, string subject, string toEmail)
        {
            try
            {
                EmailTrackerEntity emailTracker = new EmailTrackerEntity()
                {
                    EmailId = emailId,
                    Subject = subject,
                    ToEmail = toEmail,
                };
                entityContext.EmailTrackers.Add(emailTracker);
                var result = await entityContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await SharedUtility.SendEmailToModeratorAsync("TaskMan.AppInfo.TrackEmailAsync", $"email: {toEmail} " + ex.ToString());
            }
        }

        private static void SendEmailSmtp(string subject, string body, string recipient)
        {
            SmtpClient client = new SmtpClient("smtp", 80);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("email", "pwd");

            MailMessage message = new MailMessage("sender",
                           recipient,
                           subject,
                           body);

            message.From = new MailAddress(
                "sender",
                Constants.ApplicationTitle
                );
            message.IsBodyHtml = true;

            client.Send(message);
        }
    }
}