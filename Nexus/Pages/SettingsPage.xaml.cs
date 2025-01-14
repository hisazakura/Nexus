using Nexus.Services;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nexus
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CheckDependeciesButton.Click += CheckDependeciesButton_Click;
		}

        private void CheckDependeciesButton_Click(object sender, RoutedEventArgs e)
        {
            if (JavaChecker.IsJavaInstalled(out string? javaVersion))
                JavaVersion.Text = javaVersion ?? "unavailable";
            else JavaVersion.Text = "unavailable";
            if (NodeChecker.IsNodeInstalled(out string? nodeVersion))
                NodeVersion.Text = nodeVersion ?? "unavailable";
            else NodeVersion.Text = "unavailable";
        }
    }
}
