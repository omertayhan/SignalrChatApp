using ChatRooms.Security;
using Microsoft.AspNetCore.SignalR;
using SignalrChatApp.Common;
using SignalrChatApp.Data;
using SignalrChatApp.Models;
using System.Collections.Concurrent;

namespace SignalrChatApp.Hubs
{
    public class PrivateChatHub : Hub
    {
        private readonly AppDbContext _dbContext;
        private static ConcurrentDictionary<string, string> UserConnections = new ConcurrentDictionary<string, string>();

        public PrivateChatHub(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task OnConnectedAsync()
        {
            string username = Context.User.Identity.Name;
            UserConnections[Context.ConnectionId] = username;
            // Yeni bir kullanıcı bağlandığında, tüm istemcilere online kullanıcıları gönder
            return SendOnlineUsersToClient();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            UserConnections.TryRemove(Context.ConnectionId, out _);
            // Bir kullanıcı ayrıldığında, tüm istemcilere online kullanıcıları gönder
            return SendOnlineUsersToClient();
        }

        private async Task SendOnlineUsersToClient()
        {
            var onlineUsers = UserConnections.Values.Distinct();
            await Clients.All.SendAsync("ReceiveOnlineUsers", onlineUsers);
        }

        public async Task SendToUser(string sender, string receiverUserName, string message)
        {
            var senderConnectionId = Context.ConnectionId;
            var receiverConnectionId = UserConnections.FirstOrDefault(x => x.Value == receiverUserName).Key;

            if (receiverConnectionId != null)
            {
                string roomName = GetRoomName(sender, receiverUserName);
                await Groups.AddToGroupAsync(senderConnectionId, roomName);
                await Groups.AddToGroupAsync(receiverConnectionId, roomName);

                var newMessage = new Messages
                {
                    MessageId = Guid.NewGuid(),
                    Message = CryptoHelper.EncryptString(message),
                    MessageType = MessageTypes.Private.ToString(),
                    MessageGroupId = roomName,
                    SenderUser = sender,
                    ReceiverUser = receiverUserName,
                    CreatedTime = DateTime.UtcNow.AddHours(3)
                };

                _dbContext.Messages.Add(newMessage);
                await _dbContext.SaveChangesAsync();

                await Clients.Group(roomName).SendAsync("ReceiveMessage", sender, message);
            }
            else
            {
                // Alıcı kullanıcı çevrimdışı
                await Clients.Caller.SendAsync("ReceiveMessage", "System", $"{receiverUserName} is offline !!");
            }
        }

        public string GetConnectionId() => Context.ConnectionId;

        private string GetRoomName(string user1, string user2)
        {
            var orderedUsers = new List<string> { user1, user2 }.OrderBy(x => x).ToList();
            return $"room_{orderedUsers[0]}_{orderedUsers[1]}";
        }

        public async Task GetOnlineUsers()
        {
            var onlineUsers = UserConnections.Values.Distinct();
            await Clients.Caller.SendAsync("ReceiveOnlineUsers", onlineUsers);
        }
    }
}
