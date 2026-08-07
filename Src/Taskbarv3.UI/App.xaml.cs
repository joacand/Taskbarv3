using Microsoft.Extensions.DependencyInjection;
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
        public static void Main(string[] _)
        {
            using ServiceProvider provider = CreateServiceProvider();

            App app = new();
            app.InitializeComponent();

            var mainWindow = new MainWindow();
            app.MainWindow = mainWindow;
            mainWindow.Visibility = Visibility.Visible;

            var mainWindowViewModel = provider.GetRequiredService<MainWindowViewModel>();

            mainWindow.DataContext = mainWindowViewModel;
            mainWindowViewModel.OnWindowLoaded();

            app.Run();
        }

        private static ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddRegistrations();
            return services.BuildServiceProvider();
        }
    }
}
