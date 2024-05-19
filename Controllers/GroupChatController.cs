using ChatRooms.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignalrChatApp.Common;
using SignalrChatApp.Data;
using SignalrChatApp.Models;
using SignalrChatApp.ViewModels;

namespace SignalrChatApp.Controllers
{
    [Authorize]
    public class GroupChatController : Controller
    {

        private readonly AppDbContext _dbContext;

        public GroupChatController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetMessagesByGroupId([FromBody] MessagesVM model)
        {
            var messages = await _dbContext.GetMessagesByGroupId(model.MessageGroupId);

            var messagesVM = messages
                .Select(m => new MessagesVM
                {
                    MessageId = m.MessageId,
                    Message = CryptoHelper.DecryptString(m.Message),
                    MessageType = m.MessageType,
                    MessageGroupId = m.MessageGroupId,
                    SenderUser = m.SenderUser,
                    ReceiverUser = m.ReceiverUser,
                    CreatedTime = m.CreatedTime
                })
                .ToList();

            return Json(messagesVM);
        }

        [HttpPost]
        public async Task<IActionResult> SaveMessage([FromBody] MessagesVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var message = new Messages
                {
                    MessageId = Guid.NewGuid(),
                    Message = CryptoHelper.EncryptString(model.Message),
                    MessageType = MessageTypes.Group.ToString(),
                    MessageGroupId = model.MessageGroupId,
                    SenderUser = model.SenderUser,
                    ReceiverUser = null,
                    CreatedTime = model.CreatedTime
                };

                _dbContext.Messages.Add(message);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {

                return StatusCode(500, "Internal server error");
            }
        }
    }
}
