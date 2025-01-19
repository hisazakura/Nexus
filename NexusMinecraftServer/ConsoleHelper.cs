using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMinecraftServer
{
    public class ConsoleHelper
    {
        public static string? ReadLine(CancellationToken cancellationToken)
        {
            string input = string.Empty;
            while (!cancellationToken.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine("");
                        return input;
                    }
                    input += key.KeyChar;
                }
            }
            return null;
        }
    }
}
