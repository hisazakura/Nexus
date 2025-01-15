using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusMinecraftServer
{
    public class MinecraftServer(ServerConfig config)
    {
        public readonly ServerConfig Config = config;
        public Process? ServerProcess { get; private set; } = null;

        public event Action<string>? LogReceived;
        public event Action<string>? ErrorReceived;

        public void Start()
        {
            if (ServerProcess != null)
            {
                Log("[Nexus] Server is already running!");
                return;
            }

            Log("[Nexus] Starting Minecraft server...");
            try
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = "java",
                    Arguments = string.Join(" ", Config.JavaArguments),
                    WorkingDirectory = Config.WorkingDirectory,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                ServerProcess = new()
                {
                    StartInfo = startInfo,
                    EnableRaisingEvents = true
                };

                SetupProcessListeners();

                ServerProcess.Start();
                ServerProcess.BeginOutputReadLine();
                ServerProcess.BeginErrorReadLine();
            }
            catch (Exception ex)
            {
                Error($"[Nexus] Failed to start Minecraft server: {ex.Message}");
                ServerProcess = null;
            }
        }

        public void Stop()
        {
            if (ServerProcess == null)
            {
                Log("[Nexus] Server is not running!");
                return;
            }

            Log("[Nexus] Stopping Minecraft server...");
            try
            {
                using StreamWriter writer = ServerProcess.StandardInput;
                if (writer.BaseStream.CanWrite)
                    writer.WriteLine("stop");
            }
            catch (Exception ex)
            {
                Error($"[Error] Failed to send stop command to Minecraft server: {ex.Message}");
            }
        }

        private void SetupProcessListeners()
        {
            if (ServerProcess == null) return;

            ServerProcess.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
            ServerProcess.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);
            ServerProcess.Exited += (sender, e) =>
            {
                Log($"[Nexus] Minecraft server exited with code {ServerProcess.ExitCode}");
                Log("[Nexus] Server is not running.");
                ServerProcess = null;
            };
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
            LogReceived?.Invoke(message);
        }

        public void Error(string message)
        {
            Console.Error.WriteLine(message);
            ErrorReceived?.Invoke(message);
        }
    }
}
