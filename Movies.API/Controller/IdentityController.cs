using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Movies.API.Controller;

[ApiController]
[Route("api/[controller]/[action]")]
// [Authorize]
public class IdentityController : ControllerBase
{
    public IdentityController()
    {
    }

    [HttpGet]
    public IActionResult GetUserClaims()
    {
        IActionResult response = NotFound();
        try
        {
            JsonResult result = new JsonResult(
                                    from c in User.Claims
                                    select new { c.Type, c.Value }
                                );

            Console.WriteLine($"--> Claims Response : {result}");

            response = Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Exception at IdentityController > GetUserClaims : {ex.Message}");
            response = BadRequest(ex.Message);
        }
        return response;
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public IActionResult SecureEndpoint()
    {
        return Ok(new { Message = "You have accessed a secure endpoint." });
    }

    private bool ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Exception at IdentityController > ValidateToken : {ex.Message} ");
            validatedToken = null;
            return false;
        }
    }
}

enum UserClaims
{
    Type, Value
};