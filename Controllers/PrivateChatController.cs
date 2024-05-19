using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignalrChatApp.Data;
using SignalrChatApp.Models;

namespace SignalrChatApp.Controllers
{

    [Authorize]
    public class PrivateChatController : Controller
    {

        private readonly AppDbContext _context;

        public PrivateChatController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IList<Messages> MessagesList { get; set; }

        public async Task OnGetAsync(string user1, string user2)
        {
            string roomName = GetRoomName(user1, user2);
            MessagesList = await _context.GetMessagesByGroupId(roomName);
        }

        private string GetRoomName(string user1, string user2)
        {
            var orderedUsers = new List<string> { user1, user2 }.OrderBy(x => x).ToList();
            return $"room_{orderedUsers[0]}_{orderedUsers[1]}";
        }

    }
}
