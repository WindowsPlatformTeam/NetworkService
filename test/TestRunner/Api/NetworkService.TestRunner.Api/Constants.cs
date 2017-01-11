using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetworkService.TestRunner.Api
{
    public static class Constants
    {
        public const string ApiName = "API";

        public const string BaseRoute = "api";

        public const string Test = "test";
        public const string GetTestBoolean = "get-test-boolean";
        public const string GetTestBooleanWithParam = "get-test-boolean-with-param/{id}";
        public const string PostTestBoolean = "post-test-boolean";
    }
}