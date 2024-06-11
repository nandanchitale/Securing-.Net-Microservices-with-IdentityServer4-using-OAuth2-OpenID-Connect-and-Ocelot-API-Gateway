using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Movies.API.Controller;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class IdentityController : ControllerBase
{
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

            response = Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Exception at IdentityController > GetUserClaims : {ex.Message}");
            response = BadRequest(ex.Message);
        }
        return response;
    }
}

enum UserClaims
{
    Type, Value
};