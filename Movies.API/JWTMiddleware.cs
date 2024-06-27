using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

public class JWTMiddleware
{
    private readonly RequestDelegate _next;

    public JWTMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        await _next(context);
    }
}
public static class JWTTokenValidationMiddleware
{
    public static IApplicationBuilder ValidateJWTToken(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JWTMiddleware>();
    }
}