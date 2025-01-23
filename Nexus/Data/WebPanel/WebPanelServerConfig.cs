using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Data.WebPanel
{
    public class WebPanelServerConfig : INotifyPropertyChanged
    {
        private string _nexusWebPanelPath = "";
        
        public string NexusWebPanelPath
        {
            get => _nexusWebPanelPath;
            set
            {
                if (_nexusWebPanelPath != value)
                {
                    _nexusWebPanelPath = value;
                    UserSettings.Default.NexusWebPanelPath = value;
                    PropertyChanged?.Invoke(this, new(nameof(NexusWebPanelPath)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
