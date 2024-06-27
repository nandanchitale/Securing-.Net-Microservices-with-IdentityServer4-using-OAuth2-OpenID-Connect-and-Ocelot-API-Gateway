using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Movies.Client.HttpHandlers;

class AuthenticationDelegationHandler : DelegatingHandler
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationDelegationHandler(
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            string? accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.SetBearerToken(accessToken);
            }

            return await base.SendAsync(request, cancellationToken);

            //     HttpClient httpClient = _httpClientFactory.CreateClient(_configuration["IdentityServer:Id"]);
            //     TokenResponse tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(_tokenRequest);
            //     if (tokenResponse.IsError) throw new HttpRequestException("Something went wrong while requesting Access Token");
            //     request.SetBearerToken(tokenResponse.AccessToken);
            //     return await base.SendAsync(request, cancellationToken);
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