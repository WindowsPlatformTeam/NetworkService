using NetworkService.TestRunner.Api.Models;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace NetworkService.TestRunner.Api.Controllers
{
    [RoutePrefix(Constants.BaseRoute + "/" + Constants.Test)]
    public class TestController : ApiController
    {
        [HttpGet]
        [ResponseType(typeof(bool))]
        [Route(Constants.GetTestBoolean)]
        public async Task<IHttpActionResult> GetTestBoolean()
        {
            return await Task.FromResult(Ok(true));
        }

        [HttpPost]
        [ResponseType(typeof(bool))]
        [Route(Constants.PostTestBoolean)]
        public async Task<IHttpActionResult> PostTestBoolean()
        {
            return await Task.FromResult(Ok(new User() { Id = 3, Name = "prueba" }));
        }
    }
}