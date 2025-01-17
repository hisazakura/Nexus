using Microsoft.Win32;
using Nexus.Data;
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
    /// Interaction logic for MinecraftServerPage.xaml
    /// </summary>
    public partial class MinecraftServerPage : Page
    {
        public MinecraftServerPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            StartServerButton.Click += (object? sender, RoutedEventArgs e) =>
            {
                GlobalStates.MinecraftServer.Start();
            };
		}

        private void BrowseServerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = "Select Minecraft Server JAR",
                Filter = "JAR Files (*.jar)|*.jar|All Files (*.*)|*.*",
                DefaultExt = ".jar",
                CheckFileExists = true,
                CheckPathExists = true
            };

            bool? result = openFileDialog.ShowDialog();
            if (result != true) return;
            
            string selectedFile = openFileDialog.FileName;
            GlobalStates.MinecraftServer.Config.ServerPath = selectedFile;
        }

        private void SendCommand()
        {
            if (CommandTextBox.Text == null || CommandTextBox.Text.Length == 0) return;

            GlobalStates.MinecraftServer.WebsocketController.SendMessage(CommandTextBox.Text);
        }

        private void SendCommandButton_Click(object sender, RoutedEventArgs e)
        {
            SendCommand();
        }

        private void CommandTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SendCommand();
        }

        private void BrowseNexusServerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = "Select Nexus Server Executable",
                Filter = "Executable Files (*.exe)|*.exe",
                DefaultExt = ".exe",
                CheckFileExists = true,
                CheckPathExists = true
            };

            bool? result = openFileDialog.ShowDialog();
            if (result != true) return;

            string selectedFile = openFileDialog.FileName;
            GlobalStates.MinecraftServer.Config.NexusServerPath = selectedFile;
        }
    }
}
