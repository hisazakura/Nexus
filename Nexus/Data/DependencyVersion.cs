using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Data
{
    public class DependencyVersion : INotifyPropertyChanged
    {
        public enum DependecyAvailability { Available, Unavailable, Unchecked }
        private DependecyAvailability _availability = DependecyAvailability.Unchecked;
        private string? _version;
        public DependecyAvailability Availability { 
            get { return _availability; } 
            set { 
                _availability = value; 
                PropertyChanged?.Invoke(this, new(nameof(Availability))); 
            } 
        }
        public string? Version
        {
            get { return _version; }
            set
            {
                _version = value;
                PropertyChanged?.Invoke(this, new(nameof(Version)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
