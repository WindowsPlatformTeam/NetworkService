using NetworkService.Contracts.Models.Interfaces;

namespace NetworkService.Impl.Models
{
    public class ErrorModel : IError
    {
        public string Error { get; set; }

        public string GetErrorMessage()
        {
            return Error;
        }
    }
}
