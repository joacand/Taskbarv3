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

        public void ShowWindow(PopupWindow window, object dataContext, double parentLeftPosition, double parentTopPosition)
        {
            if (openWindows.ContainsKey(window))
            {
                CloseWindow(window);
            }
            switch (window)
            {
                case PopupWindow.AddShortcut:
                    ShowAddShortcut(dataContext, parentLeftPosition, parentTopPosition);
                    break;
                case PopupWindow.Settings:
                    ShowSettings(dataContext, parentLeftPosition, parentTopPosition);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(window));
            }
        }

        private void ShowAddShortcut(object dataContext, double leftPosition, double topPosition)
        {
            var addShortcutView = new AddShortcutView
            {
                DataContext = dataContext,
                Left = leftPosition
            };
            addShortcutView.Top = topPosition - addShortcutView.Height;
            openWindows.Add(PopupWindow.AddShortcut, addShortcutView);
            addShortcutView.ShowDialog();
        }

        private void ShowSettings(object dataContext, double leftPosition, double topPosition)
        {
            var settingsView = new SettingsView
            {
                DataContext = dataContext,
                Left = leftPosition
            };
            settingsView.Top = topPosition - settingsView.Height;
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
