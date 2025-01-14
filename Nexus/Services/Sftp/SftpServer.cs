using Nexus.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Services.Sftp
{
    public class SftpServer : INotifyPropertyChanged
    {
        private ServerStatus _status;
        private string _username = Environment.UserName;
        private string _password = "";

        private SftpServerController _controller;

        public ServerStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    PropertyChanged?.Invoke(this, new(nameof(Status)));
                }
            }
        }
        public string Host { get; } = "127.0.0.1";
        public int Port { get; } = 22;
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    PropertyChanged?.Invoke(this, new(nameof(Username)));
                }
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    PropertyChanged?.Invoke(this, new(nameof(Password)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public SftpServer()
        {
            _controller = new(Host, Port, Username, Password);
            ApplyEventListeners();

            _controller.Start();
        }

        private void ApplyEventListeners()
        {
            _controller.Connected += () => Status = ServerStatus.Online;
            _controller.Disconnected += () => Status = ServerStatus.Offline;

            PropertyChanged += (object? sender, PropertyChangedEventArgs e) => {
                if (e.PropertyName == nameof(Username) || e.PropertyName == nameof(Password))
                    _controller.UpdateUser(Username, Password);
            };
        }
    }
}
