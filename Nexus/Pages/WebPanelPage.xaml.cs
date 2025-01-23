using Microsoft.Win32;
using Nexus.Data;
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

namespace Nexus.Pages
{
    /// <summary>
    /// Interaction logic for WebPanelPage.xaml
    /// </summary>
    public partial class WebPanelPage : Page
    {
        public WebPanelPage()
        {
            InitializeComponent();
        }

        private void BrowseWebPanelServerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = "Select Web Panel Server Executable",
                Filter = "Executable Files (*.exe)|*.exe",
                DefaultExt = ".exe",
                CheckFileExists = true,
                CheckPathExists = true
            };

            bool? result = openFileDialog.ShowDialog();
            if (result != true) return;

            string selectedFile = openFileDialog.FileName;
            GlobalStates.WebPanelServer.Config.NexusWebPanelPath = selectedFile;
        }

        private void StartServerButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalStates.WebPanelServer.Start();
        }
    }
}
