using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TyzenR.Investor.Pages
{

    [Authorize]
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(ILogger<LogoutModel> logger)
        {
            _logger = logger;
        }
        public async Task<IActionResult> OnGet(string returnUrl = "")
        {
            await HttpContext.SignOutAsync("TyzenR.Auth");
            return LocalRedirect("~/");
        }
    }
}