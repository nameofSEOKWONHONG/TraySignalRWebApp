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

namespace TrayBackgroundApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HubBase _hubBase;
        private readonly PluginLoader _pluginLoader;
        /// <summary>
        /// <see href="https://docs.microsoft.com/ko-kr/aspnet/core/signalr/dotnet-client?view=aspnetcore-6.0&tabs=visual-studio"/>
        /// </summary>
        public MainWindow(ILogger<MainWindow> logger, HubBase hubBase, PluginLoader pluginLoader)
        {
            InitializeComponent();
            _hubBase = hubBase;
            _pluginLoader = pluginLoader;
            
            this.Loaded += async (s, e) =>
            {
                _pluginLoader.LoadFromDir(new []
                {
                    typeof(IPlugin), typeof(IHelloWorldPlugin)
                });
                
                var instance = _pluginLoader.ActivateInstance<IPlugin>();
                var result = instance.Execute();
                
                _hubBase.OnReceive<string, string>("ReceiveMessage", (user, message) =>
                {
                    this.Dispatcher.Invoke(async () =>
                    {
                        if (user is not "test") return;
                        if (message is "GetProcess")
                        {
                            var processes = Process.GetProcesses();
                            var result = string.Join(",", processes.Select(m => m.ProcessName));
                            var instance2 =  _pluginLoader.ActivateInstance<IHelloWorldPlugin>();
                            var result2 = $"{instance2.Execute()} : {result}";
                            
                            await _hubBase.SendAsync("SendMessage", new[]{"test", result2});
                        }
                        else
                        {
                            var newMessage = $"{user}: {message}";
                            MessageBox.Show(newMessage);
                        }
                    });
                });

                try
                {
                    await _hubBase.StartAsync();
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
                await _hubBase.SendAsync("SendMessage", new[]{"test", InputMessage.Text});
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}