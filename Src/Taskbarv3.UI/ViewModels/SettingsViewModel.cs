using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;
using Taskbarv3.UI.Models;

namespace Taskbarv3.UI.ViewModels
{
    internal class SettingsViewModel : BaseViewModel
    {
        private readonly IWindowService windowService;
        private readonly IFavoritesService favoritesService;
        private readonly IStatusSetter statusSetter;
        private readonly IHueService hueService;

        public ICommand RegisterHueCommand { get; set; }
        public ICommand AboutCommand { get; set; }
        public ICommand ClearFavoritesCommand { get; set; }
        public ICommand ExitProgramCommand { get; set; }

        public SettingsViewModel(
            IWindowService windowService,
            IFavoritesService favoritesService,
            IStatusSetter statusSetter,
            IHueService hueService)
        {
            this.windowService = windowService;
            this.favoritesService = favoritesService;
            this.statusSetter = statusSetter;
            this.hueService = hueService;

            RegisterHueCommand = new RelayCommand(OnRegisterHueCommand);
            AboutCommand = new RelayCommand(OnAboutCommand);
            ClearFavoritesCommand = new RelayCommand(OnClearFavoritesCommand);
            ExitProgramCommand = new RelayCommand(OnExitProgramCommand);
        }

        private void OnExitProgramCommand(object _)
        {
            Application.Current.Shutdown();
        }

        private void OnAboutCommand(object _)
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            var name = fileVersionInfo.ProductName;
            var version = fileVersionInfo.FileVersion;

            var msg = $"{name} version {version}\n" +
                $"\nTo be used on a second monitor as a second taskbar.\n" +
                $"\nDeveloped by Joacand\nProject started 2013-09-14\n";
            MessageBox.Show(msg);
        }

        private void OnClearFavoritesCommand(object obj)
        {
            favoritesService.ClearFavorites();
            statusSetter.SetStatus("Playlist cleared");
            Exit();
        }

        private async void OnRegisterHueCommand(object obj)
        {
            MessageBox.Show("Press the link button on your HUE bridge, then press OK");

            var successful = await hueService.RegisterAccount();

            if (successful)
            {
                MessageBox.Show("Registration successful");
                statusSetter.SetStatus("Registration successful");
            }
            else
            {
                MessageBox.Show("Fail to register. Did you press the link button?");
            }
        }

        private void Exit()
        {
            windowService.CloseWindow(PopupWindow.Settings);
        }
    }
}
