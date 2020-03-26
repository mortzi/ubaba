using System;
using Ursa.Abstractions.ApiExceptions;

namespace Ursa.Abstractions.ServiceCollectionExtensions
{
    public class ExceptionFactoryDelegator : IUrsaExceptionFactory
    {
        private readonly Func<UrsaApiException, Exception> _factory;
        
        public ExceptionFactoryDelegator(Func<UrsaApiException, Exception> factory)
        {
            _factory = factory;
        }

        public Exception Create(UrsaApiException ursaApiException)
        {
            return _factory(ursaApiException);
        }

        public Exception Create<TResult>(UrsaApiException<TResult> ursaApiException)
        {
            return Create((UrsaApiException) ursaApiException);
        }
    }
}