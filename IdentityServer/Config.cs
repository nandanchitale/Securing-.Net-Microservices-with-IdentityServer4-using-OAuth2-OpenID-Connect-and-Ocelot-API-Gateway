using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityServer4;

public class Config
{
    public static IEnumerable<Client> clients =>
        new List<Client>
        {
            new Client
            {
                ClientId = "moviesClient",
                ClientName = "moviesClient",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "moviesAPI" }
            },
            // New Client for MVC Application
            new Client
            {
                ClientId = "movies_mvc_client",
                ClientName = "movies_mvc_client",
                // AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedGrantTypes = GrantTypes.Hybrid,
                RequirePkce = false,
                AllowRememberConsent = false,
                RedirectUris = new List<string>() { "http://localhost:5286/signin-oidc" },
                PostLogoutRedirectUris = new List<string>()
                {
                    "http://localhost:5286/signout-callback-oidc"
                },
                ClientSecrets = new List<Secret> { new Secret("secret".Sha256()) },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Address,
                    IdentityServerConstants.StandardScopes.Email,
                    "moviesAPI",
                    "roles"
                }
            }
        };

    public static IEnumerable<ApiScope> apiScopes =>
        new ApiScope[]
        {
            // Name, Description
            new ApiScope("moviesAPI", "Movie API"),
        };

    public static IEnumerable<ApiResource> apiResources =>
        new ApiResource[]
        {
            // new ApiResource(name: "moviesAPI") { Scopes = new List<string> { "moviesAPI" } }
        };

    public static IEnumerable<IdentityResource> identityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResources.Address(),
            new IdentityResource(
                    "roles",
                    "Your role(s)",
                    new List<string>() { "role" }
            )
        };

    public static List<TestUser> testUsers =>
        new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                Username = "nandan",
                Password = "chitale",
                Claims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.GivenName, "nandan"),
                    new Claim(JwtClaimTypes.FamilyName, "chitale")
                }
            }
        };
}
