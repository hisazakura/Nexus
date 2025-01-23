using Nexus.Services.Backend;
using Nexus.Services.Minecraft;
using Nexus.Services.Ngrok;
using Nexus.Services.Sftp;
using Nexus.Services.WebPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Data
{
    public static class GlobalStates
    {
        public static readonly MinecraftServer MinecraftServer = new();
        public static readonly NgrokTunnel NgrokTunnel = new();
        public static readonly WebPanelServer WebPanelServer = new();
        public static readonly SftpServer SftpServer = new();

        public static readonly BackendService BackendService = new();

        public static readonly DependencyVersion JavaVersion = new();
        public static readonly DependencyVersion NodeVersion = new();
    }
}
