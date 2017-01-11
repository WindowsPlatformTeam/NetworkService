using NetworkService.Contracts.Models.Interfaces;
using System.Net;

namespace NetworkService.Contracts.Models.Exceptions.Basic
{
    public class NetworkServiceInformationalException<TResponse> : NetworkServiceException
    {
        private new const string Message = "Error while performing an api call 1xx. Se inner exception for details";

        public NetworkServiceInformationalException(HttpStatusCode statusCode)
            : base(Message)
        {
            StatusCode = statusCode;
        }

        public NetworkServiceInformationalException(TResponse response, HttpStatusCode statusCode)
            : base(Message)
        {
            Response = response;
            StatusCode = statusCode;
        }

        public TResponse Response { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
    }
}
