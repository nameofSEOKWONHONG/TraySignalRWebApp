using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace TrayBackgroundApp;

public class HubBase
{
    protected HubConnection _hubConnection;
    private ILogger _logger;

    public HubBase(ILogger<HubBase> logger)
    {
        _logger = logger;
    }

    public virtual async Task StartAsync()
    {
        await _hubConnection.StartAsync();
    }

    public virtual async Task StopAsync()
    {
        await _hubConnection.StopAsync();
    }
}

