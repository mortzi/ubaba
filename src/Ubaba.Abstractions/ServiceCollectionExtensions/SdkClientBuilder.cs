using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Primitives;
using Ursa.Abstractions.ApiExceptions;

namespace Ursa.Abstractions.ServiceCollectionExtensions
{
    public class SdkClientBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public SdkClientBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public SdkClientBuilder AddExceptionFactory(
            Func<UrsaApiException, Exception> factory)
        {
            _serviceCollection.AddSingleton<IUrsaExceptionFactory>(sp => 
                new ExceptionFactoryDelegator(factory));
            
            return this;
        }

        public SdkClientBuilder AddExceptionFactory(
            IUrsaExceptionFactory exceptionFactory)
        {
            return AddExceptionFactory(exceptionFactory.Create);
        }

        public void WithInterceptor<TInterceptor>()
            where TInterceptor : class, IRequestResponseInterceptor
        {
            _serviceCollection.TryAddScoped<IRequestResponseInterceptor, TInterceptor>();
        }

        public void WithInterceptor(Action<IUrsaInterceptorBuilder> configure)
        {
            var builder = new UrsaInterceptorBuilder();
            configure?.Invoke(builder);

            var headerFactories =
                new List<Func<IServiceProvider, IEnumerable<KeyValuePair<string, StringValues>?>>>(builder.HeaderFactories);

            _serviceCollection.TryAddScoped<IRequestResponseInterceptor>(serviceProvider =>
            {
                var interceptor = new DefaultRequestResponseInterceptor(serviceProvider);
                interceptor.HeaderFactories.AddRange(headerFactories);

                return interceptor;
            });
        }
    }
}