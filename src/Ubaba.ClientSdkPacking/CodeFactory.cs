using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ursa.Abstractions;
using Ursa.Abstractions.ApiExceptions;
using Ursa.ClientSdkPacking.TemplateCodeProviding;

/*
 * writer note: CodeFactory surely needs enhancements in future.
 */

namespace Ursa.ClientSdkPacking
{
    internal class CodeFactory
    {
        private readonly IEnumerable<ITemplateCodeProvider> _providers;
        private readonly PackingParameters _parameters;

        private string? _code;

        private const string RequestResponseInterceptorParameterName = "requestResponseInterceptor";
        private const string RequestResponseInterceptorMemberName = "_requestResponseInterceptor";
        private static string s_iRequestResponseInterceptorTypeFullName =>
            typeof(IRequestResponseInterceptor).FullName;
        
        private const string UrsaExceptionFactoryParameterName = "exceptionFactory";
        private const string UrsaExceptionFactoryMemberName = "_exceptionFactory";
        private static string s_iUrsaExceptionFactoryTypeFullName =>
            typeof(IUrsaExceptionFactory).FullName;


        public CodeFactory(IEnumerable<ITemplateCodeProvider> providers, PackingParameters parameters)
        {
            _providers = providers;
            _parameters = parameters;
        }

        private async Task<string> CreateTemplateAsync(CancellationToken cancellationToken)
        {
            var strBuilder = new StringBuilder();
            foreach (var provider in _providers)
            {
                var template = await provider.GetTemplateCodeAsync(cancellationToken);
                strBuilder.AppendLine(template);
            }

            return strBuilder.ToString();
        }

        public async Task<string> CreateCodeAsync(CancellationToken cancellationToken)
        {
            if (_code != null)
                return _code;

            var template = await CreateTemplateAsync(cancellationToken);
            _code = ProcessTemplate(template);

            return _code;
        }

        private string ProcessTemplate(string template)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine(template);

            var replacements = new List<(string, string)>();
            replacements.AddRange(NSwagTemplateMappings);
            replacements.AddRange(TemplateVariablesMappings);

            foreach (var (old, @new) in replacements)
            {
                strBuilder.Replace(old, @new);
            }

            return FindExceptionFactoryUsageLinesAndAddMissingParentheses(strBuilder.ToString());
        }

        private string FindExceptionFactoryUsageLinesAndAddMissingParentheses(string nonCompleteCode)
        {
            var strBuilder = new StringBuilder();
            
            var codeParts = nonCompleteCode
                .Split($"{UrsaExceptionFactoryMemberName}.Create(new {_parameters.ApiExceptionFullName}")
                .Select((s, i) =>
                    {
                        if (i == 0) // skip first code part
                            return s;

                        return $"{UrsaExceptionFactoryMemberName}.Create(new {_parameters.ApiExceptionFullName}" +
                               s.Insert(s.IndexOf(';'), ")");
                    }
                );

            foreach (var part in codeParts)
            {
                strBuilder.Append(part);
            }

            return strBuilder.ToString();
        }

        private static IEnumerable<(string, string)> TemplateVariablesMappings => new[]
        {
            ("_{{namespace}}_", UrsaApplication.PackingParameters.Namespace),
            ("_{{class}}_", UrsaApplication.PackingParameters.ClassName),
            ("_{{interceptor_member}}_", RequestResponseInterceptorMemberName),
        };

        private IEnumerable<(string, string)> NSwagTemplateMappings => new []
        {
            (
                "ApiException",
                $"{_parameters.ApiExceptionFullName}"
            ),
            (
                "ApiResponse",
                $"{_parameters.ApiResponseFullName}"
            ),
            // change name of ApiException class generated by NSwag
            (
                $"class {_parameters.ApiExceptionFullName}",
                $"class NSwagGeneratedApiException"
            ),
            // doing same for it's constructor
            (
                $"public {_parameters.ApiExceptionFullName}(",
                "public NSwagGeneratedApiException("
            ),
            // change name of ApiResponse class generated by NSwag
            (
                $"class {_parameters.ApiResponseFullName}",
                "class NSwagGeneratedApiResponse"
            ),
            // exception factory
            (
                $"new {_parameters.ApiExceptionFullName}",
                $"{UrsaExceptionFactoryMemberName}.Create(new {_parameters.ApiExceptionFullName}"
            ),
            (
                "private System.Lazy<Newtonsoft.Json.JsonSerializerSettings> _settings;",
                "private System.Lazy<Newtonsoft.Json.JsonSerializerSettings> _settings;" + 
                Environment.NewLine +
                $"private readonly {s_iRequestResponseInterceptorTypeFullName} {RequestResponseInterceptorMemberName};" + 
                Environment.NewLine + 
                $"private readonly {s_iUrsaExceptionFactoryTypeFullName} {UrsaExceptionFactoryMemberName};"
            ),
            (
                "System.Net.Http.HttpClient httpClient)",
                // "string baseUrl" +
                // ", " +
                "System.Net.Http.HttpClient httpClient" + 
                ", " +
                $"{s_iRequestResponseInterceptorTypeFullName} {RequestResponseInterceptorParameterName}" + 
                ", " +
                $"{s_iUrsaExceptionFactoryTypeFullName} {UrsaExceptionFactoryParameterName}" +
                ")"
            ),
            (
                "_httpClient = httpClient;",
                "_httpClient = httpClient;" +
                // Environment.NewLine +
                // "_baseUrl = baseUrl;" +
                Environment.NewLine + 
                $"{RequestResponseInterceptorMemberName} = {RequestResponseInterceptorParameterName};" +
                Environment.NewLine + 
                $"{UrsaExceptionFactoryMemberName} = {UrsaExceptionFactoryParameterName};"
            ),
        };
    }
}