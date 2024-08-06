using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using Taskbarv3.UI.Extensions;
using Taskbarv3.UI.ViewModels;
using Taskbarv3.UI.Views;

namespace Taskbarv3.UI
{
    public partial class App : Application
    {
        [STAThread]
        public static void Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            host.Start();

            App app = new();
            app.InitializeComponent();
            var mainWindowViewModel = host.Services.GetRequiredService<MainWindowViewModel>();
            app.MainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel,
                Visibility = Visibility.Visible
            };
            app.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
                configurationBuilder.AddUserSecrets(typeof(App).Assembly))
            .ConfigureServices((hostContext, services) =>
            {
                services.AddRegistrations();
            });
    }
}
