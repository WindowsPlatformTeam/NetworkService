using NetworkService.Contracts.Models.Exceptions.Basic;
using NetworkService.Contracts.Models.Interfaces;
using System.Net;

namespace NetworkService.Contracts.Models.Exceptions.Specifics
{
    public class NetworkServiceBadRequestException : NetworkServiceClientErrorException
    {
        public NetworkServiceBadRequestException(IError error) 
            : base(error, HttpStatusCode.BadRequest) { }
    }
}
