using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nexus.Services
{
    internal class NodeChecker
    {
        public static bool IsNodeInstalled(out string? version)
        {
            version = null;
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "node",
                    Arguments = "--version",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process
                {
                    StartInfo = processStartInfo
                };
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                version = output.Trim();

                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return false;
            }
        }
    }
}
