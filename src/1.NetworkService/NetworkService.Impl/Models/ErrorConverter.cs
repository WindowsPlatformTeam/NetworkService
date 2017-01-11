using NetworkService.Contracts.Models.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetworkService.Impl.Models
{
    public class ErrorConverter : IErrorModelConverter
    {
        public async Task<IError> ConvertError<TError>(HttpResponseMessage response) where TError : IError
        {
            IError result = null;
            if (response.Content != null)
            {
                try
                {
                    result = (await response.Content.ReadAsAsync<TError>());
                }
                catch
                {
                    var msg = (await response.Content.ReadAsStringAsync());
                    result = new ErrorModel() { Error = msg };
                }
            }
            return result;
        }
    }
}
