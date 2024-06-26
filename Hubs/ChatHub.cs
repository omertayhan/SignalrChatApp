﻿using Microsoft.AspNetCore.SignalR;

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
    }
}
