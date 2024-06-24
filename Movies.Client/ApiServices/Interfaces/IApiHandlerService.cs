namespace Movies.Client.ApiServices.Interfaces
{
    public interface IApiHandlerService
    {
        HttpClient CreateHttpClient(string ClientName);
        HttpRequestMessage PrepareRequestMessage(HttpMethod HttpMethod, string ApiEndpoint);
        Task<HttpResponseMessage> GetApiResponse(bool ConfigureAwait);
    }
}

