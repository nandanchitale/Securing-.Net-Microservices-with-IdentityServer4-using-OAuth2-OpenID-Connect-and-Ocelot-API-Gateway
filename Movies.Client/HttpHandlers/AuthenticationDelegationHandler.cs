using IdentityModel.Client;

namespace Movies.Client.HttpHandlers;

class AuthenticationDelegationHandler : DelegatingHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ClientCredentialsTokenRequest _tokenRequest;
    private readonly IConfiguration _configuration;

    public AuthenticationDelegationHandler(IConfiguration configuration, IHttpClientFactory httpClientFactory, ClientCredentialsTokenRequest clientCredentialsTokenRequest)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration)); ;
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _tokenRequest = clientCredentialsTokenRequest ?? throw new ArgumentNullException(nameof(clientCredentialsTokenRequest)); ;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            HttpClient httpClient = _httpClientFactory.CreateClient(_configuration["IdentityServer:Id"]);
            TokenResponse tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(_tokenRequest);
            if (tokenResponse.IsError) throw new HttpRequestException("Something went wrong while requesting Access Token");
            request.SetBearerToken(tokenResponse.AccessToken);
            return await base.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            string ErrorMessage = $"Exception at AuthenticationDelegationHandler > Send() => {ex.Message}";
            Console.WriteLine(ErrorMessage);
            throw new HttpRequestException(ErrorMessage);
        }
        catch (Exception ex)
        {
            string ErrorMessage = $"Exception at AuthenticationDelegationHandler > Send() => {ex.Message}";
            Console.WriteLine(ErrorMessage);
            throw new Exception(ErrorMessage);
        }
    }
}