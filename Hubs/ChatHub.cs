using Microsoft.AspNetCore.SignalR;

namespace SignalrChatApp.Hubs
{
    public class ChatHub : Hub
    {
        // Tüm kullanıcılara mesaj gönderme
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            string user = Context.User.Identity.Name;
            await Clients.All.SendAsync("ReceiveMessage", "System", $"{user} has joined the chat.");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string user = Context.User.Identity.Name;
            await Clients.All.SendAsync("ReceiveMessage", "System", $"{user} has left the chat.");
            await base.OnDisconnectedAsync(exception);
        }

        public Task SendMessageToGroup(string groupName, string message, string userName)
        {
            return Clients.Group(groupName).SendAsync("Send", $"{userName}: {message}");
        }
        public async Task AddToGroup(string groupName, string userName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Send", $"{userName} : has joined the group {groupName}!!");
        }
        public async Task RemoveFromGroup(string groupName, string userName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Send", $"{userName} has left the group {groupName}");
        }
        public Task SendPrivateMessage(string user, string message)
        {
            return Clients.User(user).SendAsync("ReceiveMessage", message);
        }

        // Tüm kullanıcılardan gelen mesajları yayınlama
        public async Task BroadcastMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
