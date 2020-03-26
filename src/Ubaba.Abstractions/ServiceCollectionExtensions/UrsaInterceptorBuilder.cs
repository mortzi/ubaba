using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Ursa.Abstractions.ServiceCollectionExtensions
{
    public class UrsaInterceptorBuilder : IUrsaInterceptorBuilder
    {
        public IList<Func<IServiceProvider, IEnumerable<KeyValuePair<string, StringValues>?>>> HeaderFactories { get; }

        public UrsaInterceptorBuilder()
        {
            HeaderFactories = new List<Func<IServiceProvider, IEnumerable<KeyValuePair<string, StringValues>?>>>();
        }

        public IUrsaInterceptorBuilder AddHttpRequestHeader(string key, StringValues value)
        {
            return AddHttpRequestHeader(_ => new KeyValuePair<string, StringValues>?[]
            {
                new KeyValuePair<string, StringValues>(key, value)
            });
        }

        public IUrsaInterceptorBuilder AddHttpRequestHeader(
            Func<IServiceProvider, IEnumerable<KeyValuePair<string, StringValues>?>> headerFactory)
        {
            HeaderFactories.Add(headerFactory);
            return this;
        }

        public IUrsaInterceptorBuilder AddAllHttpRequestHeadersFromCurrentHttpContext()
        {
            return AddHttpRequestHeader(serviceProvider =>
            {
                var myHttpContextHeaders = serviceProvider.GetRequiredService<IHttpContextAccessor>()
                    .HttpContext.Request.Headers;

                return myHttpContextHeaders
                    .Select(header => new KeyValuePair<string, StringValues>(header.Key, header.Value))
                    .Select(dummy => (KeyValuePair<string, StringValues>?) dummy)
                    .ToList();
            });
        }

        public IUrsaInterceptorBuilder AddHttpRequestHeaderFromCurrentHttpContextHeaders(string key, bool optional)
        {
            return AddHttpRequestHeader(serviceProvider =>
            {
                var myHttpContextHeaders = serviceProvider
                    .GetRequiredService<IHttpContextAccessor>().HttpContext.Request.Headers;

                var toBeAddedHeaders = new List<KeyValuePair<string, StringValues>?>();
                
                if (myHttpContextHeaders.ContainsKey(key))
                {
                    toBeAddedHeaders.Add(new KeyValuePair<string, StringValues>(key, myHttpContextHeaders[key]));
                }
                else if (!optional)
                {
                    // key was not found
                    throw new UrsaInterceptingException(
                        $"header key was not found in current {nameof(HttpContext)} header_key = {key}");
                }

                return toBeAddedHeaders;
            });
        }
    }
}