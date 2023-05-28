using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.Common;
using UsersService.Application.Interfaces;

namespace UsersService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService<JwtTokens> _authenticationService;

        public AuthenticationController(IAuthenticationService<JwtTokens> authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Authenticate(string login, string password)
        {
            var result = await _authenticationService.Authenticate(login, password);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return Unauthorized(result.Exception.Message);
        }
    }
}
