using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UsersService.Application.Common;
using UsersService.Application.DTO;
using UsersService.Application.Exceptions;
using UsersService.Application.Interfaces;

namespace UsersService.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = (await _usersService.GetActiveUsers()).Value;
            return Ok(users);
        }

        [HttpGet("age")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersByAge([FromQuery] int age)
        {
            var users = (await _usersService.GetUsersByAge(age)).Value;
            return Ok(users);
        }

        [HttpGet("{login}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserByLogin([FromRoute] string login)
        {
            var result = await _usersService.GetUserByLogin(login);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return NotFound(result.Exception.Message);
        }


        [HttpGet("current")]
        [Authorize]
        public async Task<IActionResult> CetCurrentUser()
        {
            var currentUserLogin = User.FindFirstValue(ClaimTypes.Name);

            var result = await _usersService.GetCurrentUser(currentUserLogin);

            return result.Exception switch
            {
                null => Ok(result.Value),
                NotFoundException => NotFound(result.Exception.Message),
                _ => Forbid(),
            };
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUser([FromBody] AddUserDto user)
        {
            var currentUserLogin = User.FindFirstValue(ClaimTypes.Name);
            var result = await _usersService.AddUser(user, currentUserLogin);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return BadRequest(result.Exception.Message);
        }

        [HttpPatch("{login}/password")]
        public async Task<IActionResult> UpdatePassword([FromRoute] string login, [FromBody] string newPassword)
        {
            var currentUser = Permissions.GetUserWithPermissions(login, User);
            if (currentUser == null)
            {
                return Forbid();
            }
            var result = await _usersService.UpdatePassword(login, newPassword, currentUser);

            return result.Exception switch
            {
                null => Ok(result.Value),
                NotFoundException => NotFound(result.Exception.Message),
                AccessDeniedException => Forbid(),
                _ => BadRequest(result.Exception.Message)
            };
        }

        [HttpPatch("{login}/login")]
        public async Task<IActionResult> UpdateLogin([FromRoute] string login, [FromBody] string newLogin)
        {
            var currentUser = Permissions.GetUserWithPermissions(login, User);
            if (currentUser == null)
            {
                return Forbid();
            }
            var result = await _usersService.UpdateLogin(login, newLogin, currentUser);

            return result.Exception switch
            {
                null => Ok(result.Value),
                NotFoundException => NotFound(result.Exception.Message),
                AccessDeniedException => Forbid(),
                _ => BadRequest(result.Exception.Message)
            };
        }

        [HttpPatch("{login}/info")]
        public async Task<IActionResult> UpdateInfo([FromRoute] string login, [FromBody] JsonPatchDocument patchDocument)
        {
            var currentUser = Permissions.GetUserWithPermissions(login, User);
            if (currentUser == null)
            {
                return Forbid();
            }

            var result = await _usersService.UpdateDetails(login, patchDocument, currentUser);

            return result.Exception switch
            {
                null => Ok(result.Value),
                NotFoundException => NotFound(result.Exception.Message),
                AccessDeniedException => Forbid(),
                _ => BadRequest(result.Exception.Message)
            };
        }

        [HttpDelete("{login}/hard-delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserFull([FromRoute] string login)
        {
            var result = await _usersService.DeleteUserFull(login);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return NotFound(result.Exception.Message);
        }

        [HttpDelete("{login}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserSoft([FromRoute] string login)
        {
            var currentUserLogin = User.FindFirstValue(ClaimTypes.Name);

            var result = await _usersService.DeleteUserSoft(login, currentUserLogin);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return NotFound(result.Exception.Message);
        }

        [HttpPost("{login}/undelete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreUser([FromRoute] string login)
        {
            var result = await _usersService.RestoreUser(login);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return NotFound(result.Exception.Message);
        }
    }
}
