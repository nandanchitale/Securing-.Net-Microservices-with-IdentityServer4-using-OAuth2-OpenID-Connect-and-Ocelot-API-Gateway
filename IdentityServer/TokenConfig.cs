using Microsoft.IdentityModel.Tokens;

namespace IdentityServer4;
public static class JwtTokenConfig
{
    public static SymmetricSecurityKey GetIssuerSigningKey(IConfiguration configuration)
    {
        var signingKey = configuration["SIGNING_KEY"]; // Ensure this environment variable is set
        if (string.IsNullOrEmpty(signingKey))
        {
            throw new Exception("SIGNING_KEY not configured");
        }

        return new SymmetricSecurityKey(Convert.FromBase64String(signingKey));
    }
}
