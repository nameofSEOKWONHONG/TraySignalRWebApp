using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace TrayBackgroundApp;

public class HubBase
{
    private HubConnection _hubConnection;
    private ILogger _logger;

    public HubBase(ILogger<HubBase> logger)
    {
        _logger = logger;
        _hubConnection = new HubConnectionBuilder().WithUrl("https://localhost:5004/messageHub")
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

    public async Task StartAsync()
    {
        await _hubConnection.StartAsync();
    }

    public async Task StopAsync()
    {
        await _hubConnection.StopAsync();
    }

    public void OnReceive<T1, T2>(string methodName, Action<T1, T2> action)
    {
        _hubConnection.On<T1, T2>(methodName, action);
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