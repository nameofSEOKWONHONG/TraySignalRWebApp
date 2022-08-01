using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using TrayBackgroundWeb;
using TryBackgroundApp.Hubs;

namespace TrayBackgroundApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;
        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "log"), LogEventLevel.Verbose)
                .CreateLogger();
            
            //어플리케이션에서 web hostring을 지원하기 위한 방법
            //dotnet core 부터 dotnet 6는 모두 exe프로그램이고 iis의 의존성이 없으므로 아래와 같이
            //서비스앱, winform, wpf, 아발로니아 모두 구현할 수 있다.
            //maui 및 uwp는 관련된 사항을 확인하지 못함.
            _host = Host.CreateDefaultBuilder(e.Args)
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder.UseStartup<TrayBackgroundWeb.Startup>();
                    webHostBuilder.UseUrls("http://localhost:5003", "https://localhost:5004");
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<PluginLoader>();
                    services.AddSingleton<ChatHubClient>();
                    services.AddSingleton<CommandHubClient>();
                })
                .Build();

            _host.Start();

            _host.Services.GetRequiredService<MainWindow>().Show();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host.Dispose();
            base.OnExit(e);
        }
    }
}