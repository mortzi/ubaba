using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ursa.Abstractions.ApiExceptions;
using Ursa.Abstractions.ApiResponses;
using YamlDotNet.Serialization;

namespace Ursa.ClientSdkPacking
{
    internal class PackingParameters
    {
        public string Namespace { get; private set; }
        public string ClassName { get; private set; }
        public string SwaggerJsonAddress { get; private set; }
        public string NugetSource { get; private set; }
        public IList<PackageInfo> InstallPackages { get; private set; }
        public string? OutputDirectory { get; private set; }
        public PackageInfo Package { get; private set; }

        [YamlIgnore]
        public string OutputDir { get; private set; }
        [YamlIgnore]
        public string ClientSdkCodePath { get; private set; }
        [YamlIgnore]
        public string ClientSdkCsprojPath { get; private set; }
        [YamlIgnore]
        public string NugetApiKey { get; private set; }
        [YamlIgnore]
        public string NugetPackagePath { get; private set; }
        [YamlIgnore]
        public string NugetPackageDir { get; private set;}
        [YamlIgnore]
        public string? ApiExceptionFullName { get; private set; }
        [YamlIgnore]
        public string? ApiResponseFullName { get; private set; }

        public PackingParameters()
        {
            InstallPackages = new List<PackageInfo>();
        }

        public void Initialize(string nugetApiKey)
        {
            NugetApiKey = nugetApiKey;
            var outputDir =
                OutputDirectory ?? Path.Combine(Path.GetTempPath(), "ursa", DateTime.Now.Ticks.ToString());
            
            var ursaAbstractionsPackage = new PackageInfo
            {
                PackageName = "Ubaba.Abstractions",
                PackageVersion = "0.0.1-beta2"
            };
            var annotationsPackage = new PackageInfo
            {
                PackageName = "System.ComponentModel.Annotations",
                PackageVersion = "4.7.0"
            };
            
            if (InstallPackages.All(p => p.PackageName != ursaAbstractionsPackage.PackageName))
            {
                InstallPackages.Add(ursaAbstractionsPackage);
            }

            if (InstallPackages.All(p => p.PackageName != annotationsPackage.PackageName))
            {
                InstallPackages.Add(annotationsPackage);
            }
            
            var nugetPackageDir = Path.Combine(outputDir, "nuget");
            OutputDir = outputDir;
            ClientSdkCodePath = Path.Combine(outputDir, $"{ClassName}.cs");
            ClientSdkCsprojPath = Path.Combine(outputDir, $"{Package.PackageName}.csproj");
            NugetPackageDir = nugetPackageDir;
            NugetPackagePath = Path.Combine(nugetPackageDir, $"{Package.PackageName}.{Package.PackageVersion}.nupkg");
            ApiExceptionFullName ??= typeof(UrsaApiException).FullName;
            ApiResponseFullName ??= typeof(NSwagApiResponse).FullName;
        }

        public override string ToString()
        {
            var strBuilder = new StringBuilder();

            var parametersInfo = new Dictionary<string, string>
            {
                {nameof(Namespace), Namespace},
                {nameof(ClassName), ClassName},
                {nameof(Package), Package.ToString()},
                {nameof(SwaggerJsonAddress), SwaggerJsonAddress},
                {nameof(OutputDir), OutputDir},
                {nameof(ClientSdkCodePath), ClientSdkCodePath},
                {nameof(ClientSdkCsprojPath), ClientSdkCsprojPath},
                {nameof(NugetApiKey), NugetApiKey},
                {nameof(NugetSource), NugetSource},
                {nameof(NugetPackagePath), NugetPackagePath},
                {nameof(NugetPackageDir), NugetPackageDir}
            };

            foreach (var (identifier, value) in parametersInfo)
            {
                strBuilder.AppendLine($"{identifier}: {value}");
            }

            return strBuilder.ToString();
        }
    }

    internal class PackageInfo
    {
        public string PackageName { get; set; }
        public string PackageVersion { get; set; }

        public override string ToString()
        {
            var pkgNameStr = $"{nameof(PackageName)} => {PackageName}";
            var pkgVersionStr = $"{nameof(PackageVersion)} => {PackageVersion}";

            return $"{pkgNameStr} && {pkgVersionStr}";
        }
    }
}