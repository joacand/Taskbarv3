using System.Windows.Input;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;
using Taskbarv3.UI.Models;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.IO;
using System;

namespace Taskbarv3.UI.ViewModels
{
    internal class SettingsViewModel : BaseViewModel
    {
        private readonly IWindowService windowService;
        private readonly IFavoritesService favoritesService;
        private readonly IStatusSetter statusSetter;
        private readonly IHueService hueService;
        private readonly MainConfig config;

        public ICommand StartSkypeAndSteamCommand { get; set; }
        public ICommand EditFavoritesCommand { get; set; }
        public ICommand RegisterHueCommand { get; set; }
        public ICommand AboutCommand { get; set; }
        public ICommand ClearFavoritesCommand { get; set; }
        public ICommand ExitProgramCommand { get; set; }

        public SettingsViewModel(
            IWindowService windowService,
            IConfigHandler configHandler,
            IFavoritesService favoritesService,
            IStatusSetter statusSetter,
            IHueService hueService)
        {
            this.windowService = windowService;
            this.favoritesService = favoritesService;
            this.statusSetter = statusSetter;
            this.hueService = hueService;
            config = configHandler.LoadFromFile();

            StartSkypeAndSteamCommand = new RelayCommand(OnStartSkypeAndSteamCommand);
            EditFavoritesCommand = new RelayCommand(OnEditFavoritesCommand);
            RegisterHueCommand = new RelayCommand(OnRegisterHueCommand);
            AboutCommand = new RelayCommand(OnAboutCommand);
            ClearFavoritesCommand = new RelayCommand(OnClearFavoritesCommand);
            ExitProgramCommand = new RelayCommand(OnExitProgramCommand);
        }

        private void OnStartSkypeAndSteamCommand(object _)
        {
            if (!File.Exists(config.SkypeFileName))
            {
                statusSetter.SetStatus($"Unable to find Skype at location: {config.SkypeFileName}");
                Exit();
                return;
            }
            if (!File.Exists(config.SteamFileName))
            {
                statusSetter.SetStatus($"Unable to find Steam at location: {config.SkypeFileName}");
                Exit();
                return;
            }

            ProcessStartInfo skypeProcessStartInfo = new ProcessStartInfo
            {
                WorkingDirectory = config.SkypeWorkingDirectory,
                FileName = config.SkypeFileName
            };

            ProcessStartInfo steamProcessStartInfo = new ProcessStartInfo
            {
                WorkingDirectory = config.SteamWorkingDirectory,
                FileName = config.SteamFileName
            };

            try
            {
                Process skypeProcess = Process.Start(skypeProcessStartInfo);
                Process steamProcess = Process.Start(steamProcessStartInfo);
            }
            catch (Exception e)
            {
                statusSetter.SetStatus($"Exception when opening process: {e}");
            }
            Exit();
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

        private void OnEditFavoritesCommand(object obj)
        {
            MessageBox.Show("Not implemented");
        }

        private void Exit()
        {
            windowService.CloseWindow(PopupWindow.Settings);
        }
    }
}
