using NetworkService.Contracts.Models.Interfaces;

namespace NetworkService.TestRunner.Models
{
    public class ErrorModel : IError
    {
        public string GetErrorMessage()
        {
            return "Error";
        }
    }
}
