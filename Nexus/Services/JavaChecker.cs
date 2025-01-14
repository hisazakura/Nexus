using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nexus.Services
{
    public class JavaChecker
	{
        private static Regex Matcher = new(@"""(.*)""");
		public static bool IsJavaInstalled(out string? version)
		{
			version = null;
			try
			{
                ProcessStartInfo processStartInfo = new()
				{
					FileName = "java",
					Arguments = "-version",
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

				string output = process.StandardError.ReadToEnd();
				process.WaitForExit();

				if (!output.Contains("version", StringComparison.OrdinalIgnoreCase)) return false;

				MatchCollection matches = Matcher.Matches(output);
				if (matches.Count == 0) return false;

				version = matches[0].Groups[1].Value;

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
