using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ursa.Abstractions;
using Ursa.Abstractions.ApiExceptions;
using Ursa.Abstractions.ServiceCollectionExtensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ClientSdkServiceCollectionExtensions
    {
        public static IServiceCollection AddClientSdk<TService, TImplementation>(
            this IServiceCollection serviceCollection,
            string baseUrl,
            Func<IServiceProvider, HttpClient> httpClientFactory,
            Action<SdkClientBuilder> configure)
            where TImplementation : class, TService
            where TService : class
        {
            serviceCollection.AddScoped<TService, TImplementation>(
                sp =>
                {
                    var constructorParams = new object[]
                    {
                        baseUrl,
                        httpClientFactory(sp),
                        sp.GetRequiredService<IRequestResponseInterceptor>(),
                        sp.GetRequiredService<IUrsaExceptionFactory>()
                    };
                    return (TImplementation) Activator.CreateInstance(typeof(TImplementation), constructorParams);
                });
            
            var options = new SdkClientBuilder(serviceCollection);
            configure?.Invoke(options);

            return serviceCollection;
        }

        public static IServiceCollection AddClientSdk<TService, TImplementation, TInterceptor>(
            this IServiceCollection serviceCollection,
            string baseUrl,
            Func<IServiceProvider, HttpClient> httpClientFactory)
            where TImplementation : class, TService
            where TService : class
            where TInterceptor : class, IRequestResponseInterceptor
        {
            serviceCollection.AddScoped<TService, TImplementation>(
                sp =>
                {
                    var constructorParams = new object[]
                    {
                        baseUrl,
                        httpClientFactory(sp),
                        sp.GetRequiredService<IRequestResponseInterceptor>(),
                        sp.GetRequiredService<IUrsaExceptionFactory>()
                    };
                    return (TImplementation) Activator.CreateInstance(typeof(TImplementation), constructorParams);
                });
            
            serviceCollection.TryAddScoped<IRequestResponseInterceptor, TInterceptor>();

            return serviceCollection;
        }
    }
}