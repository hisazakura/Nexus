using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Data.Minecraft
{
    public class MinecraftServerConfig : INotifyPropertyChanged
    {
        private string _nexusServerPath = "";
        private string _serverPath = "";
        private string _arguments = "-Xms4G -Xmx4G -XX:+UseG1GC";

        public string NexusServerPath
        {
            get => _nexusServerPath;
            set
            {
                if (_nexusServerPath != value)
                {
                    _nexusServerPath = value;
                    UserSettings.Default.NexusServerPath = value;
                    PropertyChanged?.Invoke(this, new(nameof(NexusServerPath)));
                }
            }
        }
        public string ServerPath
        {
            get => _serverPath;
            set
            {
                if (_serverPath != value)
                {
                    _serverPath = value;
                    UserSettings.Default.MinecraftServerPath = value;
                    PropertyChanged?.Invoke(this, new(nameof(ServerPath)));
                }
            }
        }
        public string Arguments
        {
            get => _arguments;
            set
            {
                if (_arguments != value)
                {
                    _arguments = value;
                    UserSettings.Default.MinecraftServerArguments = value;
                    PropertyChanged?.Invoke(this, new(nameof(Arguments)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
