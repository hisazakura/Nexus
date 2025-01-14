using Nexus.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for SftpServerPage.xaml
    /// </summary>
    public partial class SftpServerPage : Page
    {
        public SftpServerPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            GlobalStates.SftpServer.Password = PasswordBox.Password;
        }
    }
}
