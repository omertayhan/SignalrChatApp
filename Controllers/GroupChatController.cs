using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SignalrChatApp.Controllers
{
    [Authorize]
    public class GroupChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
