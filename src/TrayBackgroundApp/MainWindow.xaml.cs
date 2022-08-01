using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using PluginLib;
using TryBackgroundApp.Hubs;

namespace TrayBackgroundApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ChatHubClient _chatHubClient;
        private readonly CommandHubClient _commandHubClient;
        private readonly PluginLoader _pluginLoader;
        /// <summary>
        /// <see href="https://docs.microsoft.com/ko-kr/aspnet/core/signalr/dotnet-client?view=aspnetcore-6.0&tabs=visual-studio"/>
        /// </summary>
        public MainWindow(ILogger<MainWindow> logger, 
            ChatHubClient chatHubClient, 
            CommandHubClient commandHubClient, 
            PluginLoader pluginLoader)
        {
            InitializeComponent();
            _chatHubClient = chatHubClient;
            _commandHubClient = commandHubClient;
            _pluginLoader = pluginLoader;
            
            this.Loaded += async (s, e) =>
            {
                _pluginLoader.LoadFromDir(new []
                {
                    typeof(IPlugin), typeof(IHelloWorldPlugin)
                });
                
                var instance = _pluginLoader.ActivateInstance<IPlugin>();
                var result = instance.Execute();
                
                _commandHubClient.OnReceive("ReceiveCommand", (cmd) =>
                {
                    if (cmd == "GetProcess")
                    {
                        var processes = Process.GetProcesses();
                        var result = string.Join(",", processes.Select(m => m.ProcessName));
                        var instance2 =  _pluginLoader.ActivateInstance<IHelloWorldPlugin>();
                        var result2 = $"{instance2.Execute()} : {result}";
                            
                        _commandHubClient.SendCommandAsync("SendCommand", new[]{"test", result2})
                    }
                });
                _chatHubClient.OnReceive("ReceiveMessage", (user, message) =>
                {
                    this.Dispatcher.Invoke(async () =>
                    {
                        if (user is not "test") return;
                        var newMessage = $"{user}: {message}";
                        MessageBox.Show(newMessage);
                    });
                });

                try
                {
                    await _chatHubClient.StartAsync();
                    await _commandHubClient.StartAsync();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            };
        }

        private async void SendMessageClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await _chatHubClient.SendAsync("SendMessage", new[]{"test", InputMessage.Text});
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}