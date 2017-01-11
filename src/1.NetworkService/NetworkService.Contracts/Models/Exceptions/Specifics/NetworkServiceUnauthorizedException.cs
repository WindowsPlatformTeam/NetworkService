using NetworkService.Contracts.Models.Exceptions.Basic;
using NetworkService.Contracts.Models.Interfaces;
using System.Net;

namespace NetworkService.Contracts.Models.Exceptions.Specifics
{
    public class NetworkServiceUnauthorizedException : NetworkServiceClientErrorException
    {
        public NetworkServiceUnauthorizedException() : base(HttpStatusCode.Unauthorized) { }

        public NetworkServiceUnauthorizedException(IError error) : base(error, HttpStatusCode.Unauthorized) { }
    }
}
