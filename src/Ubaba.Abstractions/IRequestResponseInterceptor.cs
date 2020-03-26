using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Ursa.Abstractions
{
    public interface IRequestResponseInterceptor
    {
        void UpdateJsonSerializerSettings(JsonSerializerSettings settings);

        void PrepareRequest(HttpClient client, HttpRequestMessage request, string url);

        void PrepareRequest(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder);

        void ProcessResponse(HttpClient client, HttpResponseMessage response);
    }
}