using System;
using System.Collections.Generic;
using System.Windows;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;
using Taskbarv3.UI.Views;

namespace Taskbarv3.UI.Models
{
    internal sealed class WindowService : IWindowService
    {
        private readonly Dictionary<PopupWindow, Window> openWindows = new Dictionary<PopupWindow, Window>();

        public void ShowWindow(PopupWindow window, object dataContext)
        {
            if (openWindows.ContainsKey(window))
            {
                CloseWindow(window);
            }
            switch (window)
            {
                case PopupWindow.AddShortcut:
                    ShowAddShortcut(dataContext);
                    break;
                case PopupWindow.Settings:
                    ShowSettings(dataContext);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(window));
            }
        }

        private void ShowAddShortcut(object dataContext)
        {
            var addShortcutView = new AddShortcutView
            {
                DataContext = dataContext,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            openWindows.Add(PopupWindow.AddShortcut, addShortcutView);
            addShortcutView.ShowDialog();
        }

        private void ShowSettings(object dataContext)
        {
            var settingsView = new SettingsView
            {
                DataContext = dataContext,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            openWindows.Add(PopupWindow.Settings, settingsView);
            settingsView.ShowDialog();
        }

        public void CloseWindow(PopupWindow window)
        {
            if (openWindows.ContainsKey(window))
            {
                if (openWindows[window] != null)
                {
                    openWindows[window].Close();
                }
                openWindows.Remove(window);
            }
        }
    }
}
