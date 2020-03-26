using System.Threading;
using System.Threading.Tasks;

namespace Ursa.ClientSdkPacking
{
    internal class NugetPackageWorker
    {
        private readonly PackingParameters _packingParameters;

        public NugetPackageWorker()
        {
            _packingParameters = UrsaApplication.PackingParameters;
        }

        public async Task PackAsync(CancellationToken cancellationToken)
        {
            await Utils.RunDotnetCommand(
                $"pack {_packingParameters.ClientSdkCsprojPath} -c Release -o {_packingParameters.NugetPackageDir} -p:PackageVersion={_packingParameters.Package.PackageVersion}",
                cancellationToken);
        }

        public async Task PushAsync(CancellationToken cancellationToken)
        {
            var apiKeyOption = string.IsNullOrEmpty(_packingParameters.NugetApiKey)
                ? ""
                : $"--api-key {_packingParameters.NugetApiKey}";

            await Utils.RunDotnetCommand(
                $"nuget push --source {_packingParameters.NugetSource} {apiKeyOption} {_packingParameters.NugetPackagePath}",
                cancellationToken);
        }
    }
}