using Nexus.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for NgrokTunnelPage.xaml
    /// </summary>
    public partial class NgrokTunnelPage : Page
    {
        // TODO: Configure ngrok, add status and endpoints
        public NgrokTunnelPage()
        {
            InitializeComponent();
        }

        private static string GenerateNgrokConfiguration()
        {
           string minecraft = GlobalStates.NgrokTunnel.Config.MinecraftTunnelId;
           string web = GlobalStates.NgrokTunnel.Config.WebPanelTunnelId;
           string sftp = GlobalStates.NgrokTunnel.Config.SftpTunnelId;

            return $"version: \"3\"\r\nagent:\r\n  authtoken: <your-authtoken>\r\n\r\ntunnels:\r\n  {sftp}:\r\n    proto: tcp\r\n    addr: 22\r\n  {web}:\r\n    proto: http\r\n    addr: 3000\r\n  {minecraft}:\r\n    proto: tcp\r\n    addr: 25565";
        }

        private void GenerateConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            new CopyableMessageWindow(GenerateNgrokConfiguration(), "ngrok Configuration", true).Show();
        }

        private void StartServerButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalStates.NgrokTunnel.Start();
        }

        private void ConfigureHelpLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
