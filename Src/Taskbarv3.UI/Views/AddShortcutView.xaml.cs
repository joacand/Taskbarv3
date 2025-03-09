using System.Windows;

namespace Taskbarv3.UI.Views
{
    public partial class AddShortcutView : Window
    {
        public AddShortcutView()
        {
            InitializeComponent();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
