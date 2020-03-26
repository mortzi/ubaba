using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Ursa.Abstractions.ApiExceptions
{
    [GeneratedCode("NSwag", "13.2.2.0 (NJsonSchema v10.1.4.0 (Newtonsoft.Json v12.0.0.0))")]
    public class UrsaApiException : Exception
    {
        public int StatusCode { get; private set; }

        public string Response { get; private set; }

        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; private set; }

        public UrsaApiException(string message, int statusCode, string response, IReadOnlyDictionary<string, IEnumerable<string>> headers, Exception innerException) 
            : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + response.Substring(0, response.Length >= 512 ? 512 : response.Length), innerException)
        {
            StatusCode = statusCode;
            Response = response; 
            Headers = headers;
        }

        public override string ToString()
        {
            return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }
    
    [GeneratedCode("NSwag", "13.2.2.0 (NJsonSchema v10.1.4.0 (Newtonsoft.Json v12.0.0.0))")]
    public class UrsaApiException<TResult> : UrsaApiException
    {
        public TResult Result { get; private set; }

        public UrsaApiException(string message, int statusCode, string response, IReadOnlyDictionary<string, IEnumerable<string>> headers, TResult result, Exception innerException) 
            : base(message, statusCode, response, headers, innerException)
        {
            Result = result;
        }
    }
}