using System;

namespace Ursa.Abstractions
{
    public class UrsaInterceptingException : Exception
    {
        public UrsaInterceptingException(string message)
            : base(message)
        {
        }
    }
}