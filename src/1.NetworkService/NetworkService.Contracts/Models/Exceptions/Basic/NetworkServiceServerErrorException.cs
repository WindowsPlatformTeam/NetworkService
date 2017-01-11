using NetworkService.Contracts.Models.Interfaces;
using System.Net;

namespace NetworkService.Contracts.Models.Exceptions.Basic
{
    public class NetworkServiceServerErrorException : NetworkServiceException
    {
        private new const string Message = "Error while performing an api call 5xx. Se inner exception for details";

        public NetworkServiceServerErrorException(HttpStatusCode statusCode) : base(Message)
        {
            StatusCode = statusCode;
        }

        public NetworkServiceServerErrorException(IError error, HttpStatusCode statusCode) : 
            base(error != null ? error.GetErrorMessage() : Message)
        {
            Error = error;
            StatusCode = statusCode;
        }

        public IError Error { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
    }
}
