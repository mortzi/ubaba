using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Ursa.ClientSdkPacking
{
    [Command(Name = "ursa", Description = "A tool for creating client sdk package from swagger spec (swagger.json)")]
    internal class UrsaCommand
    {
        [Argument(0, "path to ursa.yaml file")]
        public string ConfigurationPath { get; }

        [Option(
            "--api-key",
            CommandOptionType.SingleValue
        )]
        public string NugetApiKey { get; }
        private const string DefaultUrsaConfFileName = "ursa.yaml";

        private async Task<int> OnExecuteAsync(CommandLineApplication app,
            CancellationToken cancellationToken = default)
        {
            PackingParameters packingParameters = null;
            string ursaConfPath = null;

            if (!File.Exists(ConfigurationPath) && !Directory.Exists(ConfigurationPath))
            {
                throw new FileNotFoundException(DefaultUrsaConfFileName);
            }

            var fileAttributes = File.GetAttributes(ConfigurationPath);
            if (fileAttributes.HasFlag(FileAttributes.Directory))
            {
                ursaConfPath = Directory.EnumerateFiles(ConfigurationPath)
                    .FirstOrDefault(path => path.EndsWith(DefaultUrsaConfFileName));
            }
            else
            {
                ursaConfPath = ConfigurationPath;
            }

            if (ursaConfPath == null)
            {
                throw new FileNotFoundException(DefaultUrsaConfFileName);
            }

            using (var streamReader = File.OpenText(ConfigurationPath))
            {
                packingParameters = new DeserializerBuilder()
                    .WithNamingConvention(UnderscoredNamingConvention.Instance)
                    .Build()
                    .Deserialize<PackingParameters>(streamReader);
                
                packingParameters.Initialize(NugetApiKey);
            }

            return await new UrsaApplication(app)
                .RunAsync(packingParameters, cancellationToken);
        }
    }

    internal class IsIdentifierAttribute : ValidationAttribute
    {
        public IsIdentifierAttribute()
            : base("the identifier name {0} is not valid")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || (value is string identifier &&
                                  !CodeGenerator.IsValidLanguageIndependentIdentifier(identifier)))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}