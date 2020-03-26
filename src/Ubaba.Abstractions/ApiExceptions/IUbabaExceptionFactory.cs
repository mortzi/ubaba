using System;

namespace Ursa.Abstractions.ApiExceptions
{
    public interface IUrsaExceptionFactory
    {
        Exception Create(UrsaApiException ursaApiException);

        Exception Create<TResult>(UrsaApiException<TResult> ursaApiException);
    }
}