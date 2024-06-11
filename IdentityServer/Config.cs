using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityServer4;

public class Config
{
    public static IEnumerable<Client> clients => new Client[]{
        new Client{
            ClientId="moviesClient",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = {
                new Secret("secret".Sha256())
            },
            AllowedScopes = {"MoviesAPI"}
        }
    };

    public static IEnumerable<ApiScope> apiScopes => new ApiScope[]{
        // Name, Description
        new ApiScope("MoviesAPI", "Movies API")
    };

    public static IEnumerable<ApiResource> apiResources => new ApiResource[]{

    };

    public static IEnumerable<IdentityResource> identityResources => new IdentityResource[]{

    };

    public static List<TestUser> testUsers => new List<TestUser>{

    };
}