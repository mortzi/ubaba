using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Ursa.Abstractions.ServiceCollectionExtensions
{
    public interface IUrsaInterceptorBuilder
    {
        IUrsaInterceptorBuilder AddHttpRequestHeader(string key, StringValues value);

        IUrsaInterceptorBuilder AddHttpRequestHeader(
            Func<IServiceProvider, IEnumerable<KeyValuePair<string, StringValues>?>> headerFactory);

        IUrsaInterceptorBuilder AddAllHttpRequestHeadersFromCurrentHttpContext();

        IUrsaInterceptorBuilder AddHttpRequestHeaderFromCurrentHttpContextHeaders(string key, bool optional);
    }
}