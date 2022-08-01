using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using TrayBackgroundApp;

namespace TryBackgroundApp.Hubs;

public class ChatHubClient : HubBase
{
    public ChatHubClient(ILogger<ChatHubClient> logger) : base(logger)
    {
        _hubConnection = new HubConnectionBuilder().WithUrl("https://localhost:5004/chatHub")
            //자동 재연결
            //.WithAutomaticReconnect()
            .Build();    
        
        //수동 재연결
        _hubConnection.Closed += async (err) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await _hubConnection.StartAsync();
        };
    }

    public void OnReceive(string methodName, Action<string, string> action)
    {
        _hubConnection.On<string, string>(methodName, action);
    }

    public async Task SendAsync(string methodName, object[] args)
    {
        await _hubConnection.SendCoreAsync(methodName, args);
    }

    public async Task InvokeAsync(string methodName, object[] args)
    {
        await _hubConnection.InvokeCoreAsync(methodName, args);
    }
}