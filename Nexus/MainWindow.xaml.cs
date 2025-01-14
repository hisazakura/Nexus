using Nexus.Extensions;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nexus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RadioButtonGroup SidebarRadioButtonGroup = new("SidebarRadioButtonGroup");
        private readonly Dictionary<string, Uri> ContentNavigationUri = new()
        {
            { "dashboard", new("Pages/DashboardPage.xaml", UriKind.Relative) },
            { "ngrok", new("Pages/NgrokTunnelPage.xaml", UriKind.Relative) },
            { "minecraft", new("Pages/MinecraftServerPage.xaml", UriKind.Relative) },
            { "sftp", new("Pages/SftpServerPage.xaml", UriKind.Relative) },
            { "settings", new("Pages/SettingsPage.xaml", UriKind.Relative) },
        };

		public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SidebarRadioButtonGroup.Add(SidebarRadioButtons.Children.Cast<RadioButton>());
            SidebarRadioButtonGroup.SelectionChanged += SidebarRadioButtonGroup_SelectionChanged;
		}

        private void SidebarRadioButtonGroup_SelectionChanged(RadioButton? radioButton)
        {
            if (radioButton == null || radioButton.Tag == null) return;
            if (radioButton.Tag is string tag && ContentNavigationUri.TryGetValue(tag, out Uri? uri)) 
                ContentFrame.Source = uri;
            else ContentFrame.Source = null;
		}
    }
}