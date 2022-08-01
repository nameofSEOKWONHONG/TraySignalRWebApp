using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using TrayBackgroundApp;

namespace TryBackgroundApp.Hubs;

public class CommandHubClient : HubBase
{
    public CommandHubClient(ILogger<HubBase> logger) : base(logger)
    {
        this._hubConnection = new HubConnectionBuilder().WithUrl("https://localhost:5004/commandHub").Build();
        //수동 재연결
        _hubConnection.Closed += async (err) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await _hubConnection.StartAsync();
        };
    }
    
    public void OnReceive(string methodName, Action<string> action)
    {
        _hubConnection.On<string>(methodName, action);
    }

    public async Task SendCommandAsync(string methodName, object obj)
    {
        await _hubConnection.SendAsync(methodName, obj);
    }
}