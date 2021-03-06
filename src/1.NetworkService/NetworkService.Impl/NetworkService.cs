﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NetworkService.Contracts;
using NetworkService.Contracts.Models.Interfaces;
using NetworkService.Impl.Models;
using System.Threading;
using NetworkService.Contracts.Adapters;
using NetworkService.Contracts.Models.Exceptions.Basic;
using NetworkService.Impl.Helpers;
using NetworkService.Contracts.Models.Exceptions.Specifics;
using System.Globalization;
using System.IO;
using NetworkService.Contracts.Models.Exceptions;

namespace NetworkService.Impl
{
    public sealed class NetworkService : INetworkService
    {
        #region Fields
        private readonly IHttpClientAdapter _httpClient;
        private readonly ApiToken _apiToken;
        private readonly SemaphoreSlim _semaphore;
        private IErrorModelConverter _errorConverter;
        #endregion

        #region Constructor
        public NetworkService(IHttpClientAdapter httpClient)
        {
            _httpClient = httpClient;
            _apiToken = new ApiToken();
            _semaphore = new SemaphoreSlim(1);
            _errorConverter = new ErrorConverter();
        }
        #endregion

        #region Public Methods
        public void InitToken(IToken token)
        {
            _apiToken.AddToken(token);
            _httpClient.Authorization = new AuthenticationHeaderValue(Constants.AuthHeaderSchemeBearer, _apiToken.GetToken());
        }
        public async Task<TResponse> Get<TResponse, TError>(string url)
            where TError : IError
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("URL can´t be null!");
            return await NetworkCallWrapper<TResponse, TError>(async () => await _httpClient.GetAsync(url));
        }

        public async Task<TResponse> PostWithFormUrlEncoded<TResponse, TError>(string url, IDictionary<string, string> parameters)
            where TError : IError
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("URL can´t be null!");
            if (parameters == null) throw new ArgumentNullException("Dictionary Parameters can´t be null!");

            return await NetworkCallWrapper<TResponse, TError>(async () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new FormUrlEncodedContent(parameters),
                };

                return await _httpClient.SendAsync(request);
            });
        }

        public async Task<TResponse> Post<TResponse, TRequest, TError>(string url, TRequest parameter)
            where TError : IError
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("URL can´t be null!");
            return await NetworkCallWrapper<TResponse, TError>(async () =>
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                };

                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content =
                            new StringContent(JsonConvert.SerializeObject(parameter, serializerSettings), Encoding.UTF8, Constants.MediaTypeJson)
                };
                return await _httpClient.SendAsync(request);
            });
        }

        public async Task<TResponse> Put<TResponse, TRequest, TError>(string url, TRequest parameter)
            where TError : IError
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("URL can´t be null!");
            return await NetworkCallWrapper<TResponse, TError>(async () => await _httpClient.PutAsJsonAsync(url, parameter));
        }

        public async Task<T> Delete<T, TError>(string url)
            where TError : IError
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("URL can´t be null!");
            return await NetworkCallWrapper<T, TError>(async () => await _httpClient.DeleteAsync(url));
        }

        public async Task<T> Patch<T, TError>(string url, T parameter)
            where TError : IError
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("URL can´t be null!");
            return await NetworkCallWrapper<T, TError>(async () =>
            {
                var method = new HttpMethod(Constants.MethodPatch);
                var serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var request = new HttpRequestMessage(method, url)
                {
                    Content =
                        new StringContent(JsonConvert.SerializeObject(parameter, serializerSettings), Encoding.UTF8, Constants.MediaTypeJson)
                };
                return await _httpClient.SendAsync(request);
            });
        }

        public async Task<TResponse> PostFile<TResponse, TError>(string url, IFile file)
            where TError : IError
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("URL can´t be null!");
            return await NetworkCallWrapper<TResponse, TError>(async () =>
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                };

                using (var content =
                        new MultipartFormDataContent("File-" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                {
                    content.Add(new StreamContent(new MemoryStream(file.Bytes)), file.Name, file.FileName);

                    var request = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = content
                    };
                    return await _httpClient.SendAsync(request);
                }
            });
        }

        public async Task GetAuthToken<TokenResponse>(string authUrl, IDictionary<string, string> configurationParameters)
            where TokenResponse : IToken
        {
            await _semaphore.WaitAsync();
            if (!_apiToken.IsValidToken())
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, authUrl)
                    {
                        Content = new FormUrlEncodedContent(configurationParameters)
                    };
                    var response = await _httpClient.SendAsync(request);
                    if (response != null && response.IsSuccessStatusCode)
                    {
                        _apiToken.AddToken((await response.Content.ReadAsAsync<TokenResponse>()));
                        _httpClient.Authorization = new AuthenticationHeaderValue(Constants.AuthHeaderSchemeBearer, _apiToken.GetToken());
                    }
                }
                catch (Exception exception)
                {
                    throw new NetworkServiceException(exception);
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            else
            {
                _semaphore.Release();
            }
        }

        public async Task<TResponse> NetworkCallWithoutAuthAndRetry<TResponse>(Func<Task<TResponse>> action)
        {
            return await NetworkCallWrapper(action, null, null);
        }

        public async Task<TResponse> NetworkCallRetryWithoutAuth<TResponse>(Func<Task<TResponse>> action, TimeSpan sleepPeriod, int retryCount)
        {
            var retryModel = new RetryModel
            {
                RetryCount = retryCount,
                SleepPeriod = sleepPeriod
            };
            return await NetworkCallWrapper(action, null, retryModel);
        }

        public async Task<TResponse> NetworkCallWithAuthAndRetry<TResponse>(Func<Task<TResponse>> action, Func<Task> AuthAction, TimeSpan sleepPeriod, int retryCount)
        {
            var retryModel = new RetryModel
            {
                RetryCount = retryCount,
                SleepPeriod = sleepPeriod
            };
            return await NetworkCallWrapper(action, AuthAction, retryModel);
        }
        #endregion

        #region Private Methods

        private async Task<TResponse> NetworkCallWrapper<TResponse>(Func<Task<TResponse>> action, Func<Task> AuthAction, RetryModel retryModel)
        {
            var retryCount = 2;
            var sleepPeriod = TimeSpan.FromMilliseconds(100);
            var isWithAuth = AuthAction != null;

            if (retryModel == null)
            {
                retryCount = 0;
            }
            else if (retryModel.RetryCount > 2)
            {
                retryCount = retryModel.RetryCount;
            }

            if (retryModel != null && retryModel.SleepPeriod > sleepPeriod)
            {
                sleepPeriod = retryModel.SleepPeriod;
            } 

            while (retryCount >= 0)
            {
                try
                {
                    if (isWithAuth && !_apiToken.IsValidToken())
                    {
                        await AuthAction();
                    }
                    return await action();
                }
                catch (NetworkServiceUnauthorizedException)
                {
                    _apiToken.RemoveToken();
                    --retryCount;
                    if (retryCount >= 0)
                    {
                        if (isWithAuth)
                        {
                            await AuthAction();
                        }
                        await Task.Delay(sleepPeriod);
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (NetworkServiceException)
                {
                    --retryCount;
                    if (retryCount >= 0)
                    {
                        await Task.Delay(sleepPeriod);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            throw new NetworkServiceException();
        }

        private async Task<TResponse> NetworkCallWrapper<TResponse, TError>(Func<Task<HttpResponseMessage>> action)
            where TError : IError
        {
            HttpResponseMessage response;
            try
            {
                response = await action();

                if (response != null && response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        return default(TResponse);
                    }
                    return await ReadAsAsync<TResponse>(response.Content);
                }
            }
            catch (Exception exception)
            {
                throw new NetworkServiceException(exception);
            }
            if (response != null)
            {
                await ResponseErrorAction<TResponse, TError>(response);
            }
            throw new NetworkServiceException();
        }

        private async Task ResponseErrorAction<TResponse, TError>(HttpResponseMessage response) 
            where TError : IError
        {
            var httpCodeType = response.StatusCode.GetHttpCodeTypeEnum();
            if (httpCodeType == HttpCodeGroupEnum.Informational)
            {
                TResponse result;
                if (response.Content != null)
                {
                    result = await ReadAsAsync<TResponse>(response.Content);
                }
                else
                {
                    result = default(TResponse);
                }
                throw new NetworkServiceInformationalException<TResponse>(result, response.StatusCode);
            }
            if (httpCodeType == HttpCodeGroupEnum.Redirection)
            {
                TResponse result;
                if (response.Content != null)
                {
                    result = await ReadAsAsync<TResponse>(response.Content);
                }
                else
                {
                    result = default(TResponse);
                }
                throw new NetworkServiceRedirectionException<TResponse>(result, response.StatusCode);
            }
            if (httpCodeType == HttpCodeGroupEnum.ClientError)
            {
                var error = await _errorConverter.ConvertError<TError>(response);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        throw new NetworkServiceBadRequestException(error);
                    case HttpStatusCode.NotFound:
                        throw new NetworkServiceNotFoundException(response.RequestMessage?.RequestUri?.AbsoluteUri);
                    case HttpStatusCode.Unauthorized:
                        throw new NetworkServiceUnauthorizedException(error);
                    case HttpStatusCode.Forbidden:
                        throw new NetworkServiceForbiddenException(error);
                }
                throw new NetworkServiceClientErrorException(error, response.StatusCode);
            }
            if (httpCodeType == HttpCodeGroupEnum.ServerError)
            {
                var error = await _errorConverter.ConvertError<TError>(response);
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new NetworkServiceInternalServerErrorException(error);
                }
                throw new NetworkServiceServerErrorException(error, response.StatusCode);
            }
        }

        private async Task<TResponse> ReadAsAsync<TResponse>(HttpContent content)
        {
            if (content.Headers.ContentType.MediaType == "application/json")
            {
                return await content.ReadAsAsync<TResponse>();
            }
            else
            {
                var result = Activator.CreateInstance(typeof(TResponse)) as IFile;
                if (result != null)
                {
                    result.Bytes = await content.ReadAsByteArrayAsync();
                    result.FileName = content.Headers?.ContentDisposition?.FileName?.Replace("\"", "");
                    result.Name = result.FileName.Substring(0, result.FileName.LastIndexOf('.'));
                    return (TResponse)result;
                }
                else
                {
                    throw new NetworkServiceContentTypeException();
                }
            }
        }
        #endregion
    }
}
