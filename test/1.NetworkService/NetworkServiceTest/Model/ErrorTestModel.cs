using NetworkService.Contracts.Models.Interfaces;

namespace NetworkServiceTest.Model
{
    public class ErrorTestModel : IError
    {
        public string ErrorMessage { get; set; }

        public string GetErrorMessage()
        {
            return ErrorMessage;
        }
    }
}
