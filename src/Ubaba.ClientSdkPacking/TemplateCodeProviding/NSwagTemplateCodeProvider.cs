using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NJsonSchema;
using NSwag;
using NSwag.CodeGeneration.CSharp;

namespace Ursa.ClientSdkPacking.TemplateCodeProviding
{
    internal class NSwagTemplateCodeProvider : ITemplateCodeProvider
    {
        public async Task<string> GetTemplateCodeAsync(CancellationToken cancellationToken)
        {
            var document = await CreateOpenApiDocumentAsync(cancellationToken);
            return GenerateNSwagTemplateFromDocument(document);
        }
        
        private static string GenerateNSwagTemplateFromDocument(OpenApiDocument document)
        {
            var settings = new CSharpClientGeneratorSettings
            {
                ClassName = UrsaApplication.PackingParameters.ClassName,
                GenerateClientInterfaces = true,
                CSharpGeneratorSettings =
                {
                    Namespace = UrsaApplication.PackingParameters.Namespace,
                    TypeNameGenerator = new DefaultTypeNameGenerator
                    {
                        ReservedTypeNames = new[]
                        {
                            UrsaApplication.PackingParameters.Namespace,
                            UrsaApplication.PackingParameters.ClassName
                        },
                    },
                    GenerateOptionalPropertiesAsNullable = false
                }
            };

            var generator = new CSharpClientGenerator(document, settings);
            return generator.GenerateFile();
        }

        private static async Task<OpenApiDocument> CreateOpenApiDocumentAsync(CancellationToken cancellationToken = default)
        {
            OpenApiDocument document;
            var swaggerJsonAddress = UrsaApplication.PackingParameters.SwaggerJsonAddress;

            if (Uri.TryCreate(swaggerJsonAddress, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                document = await OpenApiDocument.FromUrlAsync(swaggerJsonAddress);
            }
            else if (File.Exists(swaggerJsonAddress))
            {
                document = await OpenApiDocument.FromFileAsync(swaggerJsonAddress);
            }
            else
            {
                throw new FileNotFoundException("file could not be found", swaggerJsonAddress);
            }

            return document;
        }
    }
}