using System.IO;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Ursa.ClientSdkPacking.TemplateCodeProviding;

namespace Ursa.ClientSdkPacking
{
    internal class UrsaApplication
    {
        private readonly CommandLineApplication _commandLineApp;
        public static PackingParameters PackingParameters { get; private set; }

        public UrsaApplication(CommandLineApplication commandLineApp)
        {
            _commandLineApp = commandLineApp;
        }

        public async Task<int> RunAsync(PackingParameters parameters, CancellationToken cancellationToken)
        {
            PackingParameters = parameters;
            
            PrepareEnvironment();

            var codeFactory = new CodeFactory(new ITemplateCodeProvider[]
            {
                new NSwagTemplateCodeProvider(),
                new UrsaTemplateCodeProvider()
            }, parameters);
            var code = await codeFactory.CreateCodeAsync(cancellationToken);

            await new DotnetProjectBuilder(code)
                .BuildAsync(cancellationToken);

            var nugetWorker = new NugetPackageWorker();
            await nugetWorker.PackAsync(cancellationToken);
            await nugetWorker.PushAsync(cancellationToken);

            return 0;
        }

        private static void PrepareEnvironment()
        {
            Directory.CreateDirectory(PackingParameters.OutputDir);
        }
    }
}