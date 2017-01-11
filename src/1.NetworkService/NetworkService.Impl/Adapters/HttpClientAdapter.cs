using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NetworkService.Contracts.Adapters;

namespace NetworkService.Impl.Adapters
{
    public class HttpClientAdapter : IHttpClientAdapter
    {
        #region Fields
        private readonly HttpClient _httpClient;
        #endregion

        #region Constructor

        public HttpClientAdapter()
        {
            _httpClient = new HttpClient();
        }

        #endregion

        #region Properties

        public AuthenticationHeaderValue Authorization
        {
            get
            {
                return _httpClient.DefaultRequestHeaders?.Authorization;
            }

            set
            {
                _httpClient.DefaultRequestHeaders.Authorization = value;
            }
        }

        #endregion

        public Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            return _httpClient.DeleteAsync(requestUri);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return _httpClient.GetAsync(requestUri);
        }

        public Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T value)
        {
            return _httpClient.PutAsJsonAsync(requestUri, value);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return _httpClient.SendAsync(request);
        }
    }
}
