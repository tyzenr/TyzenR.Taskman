using TyzenR.Account.Entity;

namespace TyzenR.Taskman.Managers
{
    public interface IAppInfo
    {
        Guid CurrentUserId { get; }

        DateTime GetCurrentDateTime();

        UserEntity GetCurrentUser();

        string CurrentTimeZoneId { get; }

        string CurrentUserIPAddress { get; }

        DateTime ConvertToUtc(DateTime date);

        Task SendEmailAsync(
            string toEmail,
            string subject,
            string body,
            bool tracking = true,
            List<string> attachments = null);
    }
}