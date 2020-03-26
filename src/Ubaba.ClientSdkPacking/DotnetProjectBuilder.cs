using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Ursa.ClientSdkPacking
{
    internal class DotnetProjectBuilder
    {
        private readonly PackingParameters _packingParameters;
        private readonly string _code;
        
        public DotnetProjectBuilder(string code)
        {
            _packingParameters = UrsaApplication.PackingParameters;
            _code = code;
        }

        private async Task NewProjectAsync(CancellationToken cancellationToken)
        {
            await Utils.RunDotnetCommand(
                $"new classlib -n {_packingParameters.Package.PackageName} -o {_packingParameters.OutputDir} -f netstandard2.0" /*+ "--no-restore"*/, // no restore option even slowed it down :|
                cancellationToken);
            
            //remove default generated .cs files
            File.Delete(Path.Combine(_packingParameters.OutputDir, "Class1.cs"));
        }
        
        private async Task AddCodeToProjectAsync(CancellationToken cancellationToken)
        {
            await using var writer = File.CreateText(_packingParameters.ClientSdkCodePath);
            
            await writer.WriteLineAsync(_code);
        }

        private async Task AddPackages(CancellationToken cancellationToken)
        {
            const string addPackageCommand = "add {0} package {1} {2} --no-restore";

            foreach (var packageInfo in _packingParameters.InstallPackages)
            {
                var versionOption = string.IsNullOrEmpty(packageInfo.PackageVersion)
                    ? "" // when no version is specified it defaults to latest version
                    : $"--version {packageInfo.PackageVersion}";

                // can run in parallel? not sure now
                await Utils.RunDotnetCommand(
                    string.Format(addPackageCommand, _packingParameters.ClientSdkCsprojPath, packageInfo.PackageName,
                        versionOption), cancellationToken);
            }
        }
        
        private async Task RestoreProjectAsync(CancellationToken cancellationToken)
        {
            await Utils.RunDotnetCommand(
                $"restore {_packingParameters.ClientSdkCsprojPath}",
                cancellationToken);
        }
        
        private async Task BuildProjectAsync(CancellationToken cancellationToken)
        {
            await Utils.RunDotnetCommand(
                $"build {_packingParameters.ClientSdkCsprojPath} -c Release",
                cancellationToken);
        }

        public async Task BuildAsync(CancellationToken cancellationToken)
        {
            await NewProjectAsync(cancellationToken);
            await AddCodeToProjectAsync(cancellationToken);
            await AddPackages(cancellationToken);
            await RestoreProjectAsync(cancellationToken);
            await BuildProjectAsync(cancellationToken);  
        }
    }
}
