using NetworkService.Contracts.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetworkService.Contracts
{
    public interface INetworkService
    {
        void InitToken(IToken token);
        Task<TResponse> Get<TResponse, TError>(string url)
            where TError : IError;
        Task<TResponse> PostWithFormUrlEncoded<TResponse, TError>(string url, IDictionary<string, string> parameters)
            where TError : IError;
        Task<TResponse> Post<TResponse, TRequest, TError>(string url, TRequest parameter)
            where TError : IError;
        Task<TResponse> Put<TResponse, TRequest, TError>(string url, TRequest parameter)
            where TError : IError;
        Task<T> Delete<T, TError>(string url)
            where TError : IError;
        Task<T> Patch<T, TError>(string url, T parameter)
            where TError : IError;
        Task<TResponse> PostFile<TResponse, TError>(string url, IFile file)
            where TError : IError;
        Task GetAuthToken<TokenResponse>(string authUrl, IDictionary<string, string> configurationParameters)
            where TokenResponse : IToken;
        Task<TResponse> NetworkCallWithoutAuthAndRetry<TResponse>(Func<Task<TResponse>> action);
        Task<TResponse> NetworkCallRetryWithoutAuth<TResponse>(Func<Task<TResponse>> action, TimeSpan sleepPeriod, int retryCount);
        Task<TResponse> NetworkCallWithAuthAndRetry<TResponse>(Func<Task<TResponse>> action, Func<Task> AuthAction, TimeSpan sleepPeriod, int retryCount);
    }
}
