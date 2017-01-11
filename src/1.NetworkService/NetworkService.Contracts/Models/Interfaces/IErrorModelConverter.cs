using System.Net.Http;
using System.Threading.Tasks;

namespace NetworkService.Contracts.Models.Interfaces
{
    public interface IErrorModelConverter
    {
        Task<IError> ConvertError<TError>(HttpResponseMessage response) where TError : IError;
    }
}
