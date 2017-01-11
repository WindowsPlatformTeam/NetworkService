using NetworkService.Impl.Models;
using System.Net;

namespace NetworkService.Impl.Helpers
{
    public static class HttpCodeExtensions
    {
        public static HttpCodeGroupEnum GetHttpCodeTypeEnum(this HttpStatusCode httpCode)
        {
            var result = HttpCodeGroupEnum.None;
            var code = (int)httpCode;
            if(100 <= code && code < 200)
            {
                result = HttpCodeGroupEnum.Informational;
            }
            else if (200 <= code && code < 300)
            {
                result = HttpCodeGroupEnum.Successful;
            }
            else if (300 <= code && code < 400)
            {
                result = HttpCodeGroupEnum.Redirection;
            }
            else if (400 <= code && code < 500)
            {
                result = HttpCodeGroupEnum.ClientError;
            }
            else if (500 <= code)
            {
                result = HttpCodeGroupEnum.ServerError;
            }

            return result;
        }
    }
}
