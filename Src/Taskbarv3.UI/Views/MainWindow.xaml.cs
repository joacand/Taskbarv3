using PubSub;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Taskbarv3.Core.Models;
using Taskbarv3.Core.Models.Events;

namespace Taskbarv3.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                Task.Run(() =>
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (e.MiddleButton == MouseButtonState.Pressed && sender is Button button)
                        {
                            var data = new DataObject();
                            data.SetData("Source", button.DataContext);
                            DragDrop.DoDragDrop(button, data, DragDropEffects.Move);
                            e.Handled = true;
                        }
                    }), null);
                }, CancellationToken.None);
            }
        }

        private void Button_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData("Source") is Shortcut source && sender is Button button && button.DataContext is Shortcut shortCut)
            {
                var newIndex = shortcutsItemsControl.Items.IndexOf(shortCut);
                var list = shortcutsItemsControl.ItemsSource as ObservableCollection<Shortcut>;
                list.RemoveAt(list.IndexOf(source));
                list.Insert(newIndex, source);
                shortCut.Index = newIndex;
                Hub.Default.Publish<ShortcutModifiedEvent>();
            }
        }
    }
}
