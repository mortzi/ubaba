using System.Threading;
using System.Threading.Tasks;

namespace Ursa.ClientSdkPacking.TemplateCodeProviding
{
    internal class UrsaTemplateCodeProvider : ITemplateCodeProvider
    {
        // escaped code using https://www.freeformatter.com/java-dotnet-escape.html

        private const string RequestResponseDelegatorTemplate = @"
                /*

                    *** Delegator ***

                */
                namespace _{{namespace}}_
                {
                    public partial class _{{class}}_
                    {
                        partial void UpdateJsonSerializerSettings(Newtonsoft.Json.JsonSerializerSettings settings)
                        {
                            _{{interceptor_member}}_.UpdateJsonSerializerSettings(settings);
                        }

                        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request,
                            string url)
                        {
                            _{{interceptor_member}}_.PrepareRequest(client, request, url);
                        }

                        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request,
                            System.Text.StringBuilder urlBuilder)
                        {
                            _{{interceptor_member}}_.PrepareRequest(client, request, urlBuilder);
                        }

                        partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response)
                        {
                            _{{interceptor_member}}_.ProcessResponse(client, response);
                        }
                    }
                }";

        public Task<string> GetTemplateCodeAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(RequestResponseDelegatorTemplate);
        }
    }
}