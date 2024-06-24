using Movies.Client.ApiServices.Interfaces;

namespace Movies.Client.ApiServices.Implementation
{
    public class ApiHandlerService : IApiHandlerService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpRequestMessage _requestMessage;
        private HttpResponseMessage _responseMessage;
        private HttpClient _httpClient;
        public ApiHandlerService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public HttpClient CreateHttpClient(string ClientName)
        {
            try
            {
                _httpClient = _httpClientFactory.CreateClient(ClientName);
                return _httpClient;
            }
            catch (Exception ex)
            {
                string ErrorMessage = $"--> Exception at ApiHandlerService > CreateHttpClient : {ex.Message}";
                Console.WriteLine(ErrorMessage);
                throw new Exception(ErrorMessage);
            }
        }

        public async Task<HttpResponseMessage> GetApiResponse(bool ConfigureAwait)
        {
            try
            {
                _responseMessage = await _httpClient.SendAsync(
                    _requestMessage,
                    HttpCompletionOption.ResponseHeadersRead
                ).ConfigureAwait(ConfigureAwait);
                return _responseMessage;
            }
            catch (Exception ex)
            {
                string ErrorMessage = $"--> Exception at ApiHandlerService > GetApiResponse : {ex.Message}";
                Console.WriteLine(ErrorMessage);
                throw new Exception(ErrorMessage);
            }
        }

        public HttpRequestMessage PrepareRequestMessage(HttpMethod HttpMethod, string ApiEndpoint)
        {
            try
            {
                _requestMessage = new HttpRequestMessage(HttpMethod, ApiEndpoint);
                return _requestMessage;
            }
            catch (Exception ex)
            {
                string ErrorMessage = $"--> Exception at ApiHandlerService > PrepareRequestMessage : {ex.Message}";
                Console.WriteLine(ErrorMessage);
                throw new Exception(ErrorMessage);
            }
        }
    }
}