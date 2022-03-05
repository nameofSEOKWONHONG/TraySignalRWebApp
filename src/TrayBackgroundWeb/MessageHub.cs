using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;

namespace TrayBackgroundWeb;

public class MessageHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}