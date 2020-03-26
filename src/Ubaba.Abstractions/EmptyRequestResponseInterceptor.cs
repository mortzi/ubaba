using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Ursa.Abstractions
{
    public class EmptyRequestResponseInterceptor : IRequestResponseInterceptor
    {
        public void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
        }

        public void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
        {
        }

        public void PrepareRequest(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder)
        {
        }

        public void ProcessResponse(HttpClient client, HttpResponseMessage response)
        {
        }
    }
}