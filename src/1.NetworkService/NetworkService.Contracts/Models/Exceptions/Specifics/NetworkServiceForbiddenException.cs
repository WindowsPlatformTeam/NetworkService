using NetworkService.Contracts.Models.Exceptions.Basic;
using NetworkService.Contracts.Models.Interfaces;
using System.Net;

namespace NetworkService.Contracts.Models.Exceptions.Specifics
{
    public class NetworkServiceForbiddenException : NetworkServiceClientErrorException
    {
        public NetworkServiceForbiddenException() : base(HttpStatusCode.Forbidden) { }

        public NetworkServiceForbiddenException(IError error) : base(error, HttpStatusCode.Forbidden) { }
    }
}
