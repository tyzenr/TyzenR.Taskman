using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TyzenR.Account.Common;

namespace TyzenR.Taskman.Pages
{

    [Authorize]
    public class LogoutModel : PageModel
    {
        private readonly AppSettings appSettings;

        public LogoutModel(ILogger<LogoutModel> logger, AppSettings appSettings)
        {
            this.appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public async Task<IActionResult> OnGet(string returnUrl = "")
        {
            await HttpContext.SignOutAsync(appSettings.AuthSchemeName);
            return LocalRedirect("~/");
        }
    }
}