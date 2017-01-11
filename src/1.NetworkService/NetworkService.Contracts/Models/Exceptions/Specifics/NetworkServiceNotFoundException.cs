using NetworkService.Contracts.Models.Exceptions.Basic;
using System.Net;

namespace NetworkService.Contracts.Models.Exceptions.Specifics
{
    public class NetworkServiceNotFoundException : NetworkServiceClientErrorException
    {
        public NetworkServiceNotFoundException(string url) : base(HttpStatusCode.NotFound) { }

        public string Url { get; set; }
    }
}
