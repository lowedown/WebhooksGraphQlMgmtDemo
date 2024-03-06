using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sitecore.XM.AuthWebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            string refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            return Content($"Current user: <span id=\"UserIdentityName\">{User.Identity?.Name ?? "anonymous"}</span><br/>" +
                $"<div>Access token: {accessToken}</div><br/>" +
                $"<div>Refresh token: {refreshToken}</div><br/>"
                , "text/html");
        }

    }
}
