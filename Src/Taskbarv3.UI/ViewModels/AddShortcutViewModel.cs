using Microsoft.Win32;
using PubSub;
using System.Windows.Input;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;
using Taskbarv3.Core.Models.Events;
using Taskbarv3.UI.Models;

namespace Taskbarv3.UI.ViewModels
{
    internal class AddShortcutViewModel : BaseViewModel
    {
        private readonly IWindowService windowService;

        private string shortcutName;
        private string shortcutPath;
        private string shortcutIconPath;

        public ICommand BrowsePathCommand { get; set; }
        public ICommand BrowseIconCommand { get; set; }
        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public string ShortcutName { get => shortcutName; set => SetProperty(ref shortcutName, value); }
        public string ShortcutPath { get => shortcutPath; set => SetProperty(ref shortcutPath, value); }
        public string ShortcutIconPath { get => shortcutIconPath; set => SetProperty(ref shortcutIconPath, value); }

        public AddShortcutViewModel(IWindowService windowService)
        {
            this.windowService = windowService;

            BrowsePathCommand = new RelayCommand(OnBrowsePathCommand);
            BrowseIconCommand = new RelayCommand(OnBrowseIconCommand);
            OkCommand = new RelayCommand(OnOkCommand);
            CancelCommand = new RelayCommand(OnCancelCommand);
        }

        private void OnCancelCommand(object _)
        {
            windowService.CloseWindow(PopupWindow.AddShortcut);
        }

        private void OnOkCommand(object _)
        {
            var workingDir = "";
            var path = ShortcutPath.Replace("\"", "").Trim();
            if (string.IsNullOrWhiteSpace(path))
            {
                windowService.CloseWindow(PopupWindow.AddShortcut);
            }
            string[] pathSplit = path.Split('\\');
            if (!path.Equals("") && pathSplit.Length > 1)
            {
                workingDir = path.Replace(pathSplit[pathSplit.Length - 1], "");
            }
            Hub.Default.Publish(new ShortcutAddedEvent(new ShortcutMetaData(ShortcutName ?? string.Empty, path, ShortcutIconPath, workingDir)));
            windowService.CloseWindow(PopupWindow.AddShortcut);
        }

        private void OnBrowseIconCommand(object _)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Title = "Open Image File",
                Filter = "Image Files|*.bmp;*.gif;*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
                InitialDirectory = @"C:\"
            };

            var successful = fileDialog.ShowDialog() ?? false;
            if (successful)
            {
                ShortcutIconPath = fileDialog.FileName;
            }
        }

        private void OnBrowsePathCommand(object _)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Title = "Open EXE File",
                Filter = "EXE Files|*.exe",
                InitialDirectory = @"C:\"
            };

            var successful = fileDialog.ShowDialog() ?? false;
            if (successful)
            {
                ShortcutPath = fileDialog.FileName;
            }
        }
    }
}
