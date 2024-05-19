using ChatRooms.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignalrChatApp.Data;
using SignalrChatApp.Models;
using SignalrChatApp.ViewModels;

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

        [HttpPost]
        public async Task<JsonResult> GetMessages([FromBody] MessagesVM model)
        {
            string roomName = GetRoomName(model.SenderUser, model.ReceiverUser);
            var messages = await _context.GetMessagesByPrivateGroupId(roomName);

            // Mesajları MessagesViewModel listesine dönüştür
            var messagesViewModel = messages.Select(m => new MessagesVM
            {
                MessageId = m.MessageId,
                Message = CryptoHelper.DecryptString(m.Message),
                MessageType = m.MessageType,
                MessageGroupId = m.MessageGroupId,
                SenderUser = m.SenderUser,
                ReceiverUser = m.ReceiverUser,
                CreatedTime = m.CreatedTime
            }).ToList();

            return new JsonResult(messagesViewModel);
        }

        private string GetRoomName(string user1, string user2)
        {
            var orderedUsers = new List<string> { user1, user2 }.OrderBy(x => x).ToList();
            return $"room_{orderedUsers[0]}_{orderedUsers[1]}";
        }

    }
}
