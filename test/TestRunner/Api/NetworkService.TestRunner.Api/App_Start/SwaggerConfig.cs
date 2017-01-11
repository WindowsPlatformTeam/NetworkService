using System.Web.Http;
using Swashbuckle.Application;


namespace NetworkService.TestRunner.Api
{
    public class SwaggerConfig
    {
        public static void Configure(HttpConfiguration config)
        {
            config
            .EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", Constants.ApiName);
                c.Schemes(new[] { "http", "https" });
            })
          .EnableSwaggerUi();
        }
    }
}
