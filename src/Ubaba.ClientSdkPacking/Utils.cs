using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Ursa.ClientSdkPacking
{
    internal static class Utils
    {
        public static async Task RunDotnetCommand(string args, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                var process = Process.Start(DotNetExe.FullPathOrDefault(), args);
                process?.WaitForExit((int)TimeSpan.FromSeconds(30).TotalMilliseconds);
            }, cancellationToken);
        }
    }
}