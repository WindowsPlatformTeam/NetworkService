using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using NetworkService.Impl.Models;
using NetworkService.Impl.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetworkServiceTest.Helpers
{
    public static class GenerateRespondeHelper
    {
        public static HttpResponseMessage GetResponseOK(HttpStatusCode okCode)
        {
            var code = (int)okCode;
            if (100 <= code && code < 400)
            {
                var content = new StringContent($"{{'name' : '{TestConstanst.TEST_USERNAME}'}}", Encoding.UTF8, "application/json");
                return new HttpResponseMessage
                {
                    Content = okCode != HttpStatusCode.NoContent ? content : null,
                    StatusCode = okCode
                };
            }
            else throw new InternalTestFailureException();
        }

        public static HttpResponseMessage GetResponseError(HttpStatusCode code)
        {
            return new HttpResponseMessage
            {
                Content = new StringContent($"{{'errorMessage' : '{TestConstanst.TEST_ERROR_MESSAGE}'}}", Encoding.UTF8, "application/json"),
                StatusCode = code
            };
        }

        public static HttpResponseMessage GetResponseErrorNull(HttpStatusCode code)
        {
            return new HttpResponseMessage
            {
                Content = null,
                StatusCode = code
            };
        }

        public static List<HttpStatusCode> GetRangeCodes(HttpCodeGroupEnum groupHttp)
        {
            var codes = Enum.GetValues(typeof(HttpStatusCode));
            var listCodes = new List<HttpStatusCode>();

            foreach (HttpStatusCode httpCode in codes)
            {
                var code = (int)httpCode;
                if (httpCode.GetHttpCodeTypeEnum() == groupHttp)
                {
                    listCodes.Add(httpCode);
                }
            }
            return listCodes;
        }
    }
}
