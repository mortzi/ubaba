using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Ursa.ClientSdkPacking
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) => cancellationTokenSource.Cancel();

            await CommandLineApplication.ExecuteAsync<UrsaCommand>(args, cancellationTokenSource.Token);
        }
    }
}