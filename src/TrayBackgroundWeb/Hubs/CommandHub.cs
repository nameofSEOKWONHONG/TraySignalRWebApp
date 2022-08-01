using Microsoft.AspNetCore.SignalR;

namespace TrayBackgroundWeb;

public class CommandHub : Hub
{
    public async Task SendCommand(string cmd)
    {
        await Clients.Client(this.Context.ConnectionId).SendAsync("ReceiveCommand", cmd);
    }
}