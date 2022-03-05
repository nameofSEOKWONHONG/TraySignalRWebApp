using System;
using System.Collections.Generic;
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

namespace TrayBackgroundApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HubConnection _hubConnection;
        /// <summary>
        /// <see href="https://docs.microsoft.com/ko-kr/aspnet/core/signalr/dotnet-client?view=aspnetcore-6.0&tabs=visual-studio"/>
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += async (s, e) =>
            {
                _hubConnection = new HubConnectionBuilder().WithUrl("https://localhost:5001/messageHub")
                    //자동 재연결 - 설정은 href참조
                    //.WithAutomaticReconnect()
                    .Build();
                
                //수동 재연결
                _hubConnection.Closed += async (err) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await _hubConnection.StartAsync();
                };
                
                _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        var newMessage = $"{user}: {message}";
                        MessageBox.Show(newMessage);
                    });
                });

                try
                {
                    await _hubConnection.StartAsync();
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
                await _hubConnection.InvokeAsync("SendMessage", 
                    "test", InputMessage.Text);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}