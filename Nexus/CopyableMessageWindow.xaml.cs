using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Nexus
{
    /// <summary>
    /// Interaction logic for CopyableMessageWindow.xaml
    /// </summary>
    public partial class CopyableMessageWindow : Window
    {
        private string _title;
        private string _message;
        private bool _selectOnFocus;

        public CopyableMessageWindow(string message, string title, bool selectOnFocus = false)
        {
            InitializeComponent();
            _message = message;
            _title = title;
            _selectOnFocus = selectOnFocus;
        }

        private void ApplyProperties()
        {
            this.Title = _title;
            MessageTextBox.Text = _message;

            if (_selectOnFocus)
            {
                MessageTextBox.PreviewMouseLeftButtonDown += SelectivelyIgnoreMouseButton;
                MessageTextBox.GotKeyboardFocus += SelectAllText;
                MessageTextBox.MouseDoubleClick += SelectAllText;
            }
        }

        private static void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            // Find the TextBox
            DependencyObject? parent = e.OriginalSource as UIElement;
            while (parent != null && parent is not TextBox)
                parent = VisualTreeHelper.GetParent(parent);

            if (parent != null)
            {
                var textBox = (TextBox)parent;
                if (!textBox.IsKeyboardFocusWithin)
                {
                    // If the text box is not yet focussed, give it the focus and
                    // stop further processing of this click event.
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }

        private static void SelectAllText(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TextBox textBox) textBox.SelectAll();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyProperties();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
