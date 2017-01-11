using NetworkService.Contracts.Models.Interfaces;
using System;
using System.Net;

namespace NetworkService.Contracts.Models.Exceptions.Basic
{
    public class NetworkServiceClientErrorException : NetworkServiceException
    {
        private new const string Message = "Error while performing an api call 4xx. Se inner exception for details";

        public NetworkServiceClientErrorException(HttpStatusCode statusCode) : base(Message)
        {
            StatusCode = statusCode;
        }

        public NetworkServiceClientErrorException(IError error, HttpStatusCode statusCode) : 
            base(error != null ? error.GetErrorMessage() : Message)
        {
            Error = error;
            StatusCode = statusCode;
        }

        public IError Error { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
    }
}
