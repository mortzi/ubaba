using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Ursa.Abstractions
{
    public class DefaultRequestResponseInterceptor : IRequestResponseInterceptor
    {
        private readonly IServiceProvider _serviceProvider;
        public List<Func<IServiceProvider, IEnumerable<KeyValuePair<string, StringValues>?>>> HeaderFactories { get; }

        public DefaultRequestResponseInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            HeaderFactories = new List<Func<IServiceProvider, IEnumerable<KeyValuePair<string, StringValues>?>>>();
        }

        public void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var factory in HeaderFactories)
            {
                var headers = factory(_serviceProvider);

                foreach (var header in headers)
                {
                    if (!header.HasValue)
                        continue;

                    request.Headers.TryAddWithoutValidation(header.Value.Key, header.Value.Value.ToArray());
                }
            }
        }

        public void PrepareRequest(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder)
        {
        }

        public void ProcessResponse(HttpClient client, HttpResponseMessage response)
        {
            // indra is using it
            
            // var transformed = new ApiResponse<T>(response.StatusCode,
            //     new Multimap<string, string>(), result, await response.Content.ReadAsStringAsync())
            // {
            //     ErrorText = response.ReasonPhrase,
            //     Cookies = new List<Cookie>()
            // };
            
            // if (response.Headers != null)
            // {
            //     foreach (var responseHeader in response.Headers)
            //     {
            //         transformed.Headers.Add(responseHeader.Key, ClientUtils.ParameterToString(responseHeader.Value));
            //     }
            // }
        }

        public void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
        }
    }
}