using System.Runtime.Serialization;

namespace NetworkService.Contracts.Models.Interfaces
{
    public interface IError
    {
        string GetErrorMessage();
    }
}
