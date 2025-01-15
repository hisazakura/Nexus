using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusMinecraftServer
{
    public class ServerConfig
    {
        public readonly string ServerPath;
        public readonly string WorkingDirectory;
        public readonly string[] JavaArguments;

        public ServerConfig(string[] args)
        {
            ServerPath = args.Length > 0 ? args[0] : "./server.jar";
            WorkingDirectory = Path.GetDirectoryName(ServerPath) ?? ".";
            JavaArguments = [.. (args.Length > 1 ? args[1..] : ["-Xmx2G", "-Xms2G"]), 
                "-jar ", Path.GetFileName(ServerPath), "--nogui"];
        }

        public void Validate()
        {
            if (!File.Exists(ServerPath))
                throw new FileNotFoundException($"Server JAR not found at {ServerPath}");
        }
    }
}
