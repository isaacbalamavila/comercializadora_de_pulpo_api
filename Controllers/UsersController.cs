using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.User;
using comercializadora_de_pulpo_api.Services.Interfaces;
using comercializadora_de_pulpo_api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace comercializadora_de_pulpo_api.Controllers
{
    [Route("/users")]
    [Authorize]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        private IActionResult HandleResponse<T>(Response<T> response, object? successData = null)
        {
            if (response.IsSuccess)
                return response.StatusCode switch
                {
                    204 => NoContent(),
                    201 => StatusCode(201, successData ?? response.Data),
                    _ => Ok(successData ?? response.Data),
                };

            return response.StatusCode switch
            {
                400 => BadRequest(response.Error),
                404 => NotFound(response.Error),
                _ => StatusCode(500, response.Error),
            };
        }

        [HttpGet]
        [Authorize(Policy = RoleAccess.ADMINORMANAGER)]
        public async Task<IActionResult> GetUsers([FromQuery]bool all = false)
        {
            string userId = Request.Headers ["userId"].ToString();
            return HandleResponse(await _userService.GetUsersAsync(userId, all));
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            return HandleResponse(await _userService.GetUserDetailsByIdAsync(id));
        }

        [HttpPost]
        [Authorize(Policy = RoleAccess.ADMIN)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO body)
        {
            return HandleResponse(await _userService.CreateUserAsync(body));
        }

        [HttpPut("{id:Guid}")]
        [Authorize(Policy = RoleAccess.ADMIN)]
        public async Task<IActionResult> UpdateUserInfo(Guid id, [FromBody] UpdateUserDTO body)
        {
            return HandleResponse(await _userService.UpdateUserAsync(id, body));
        }

        [HttpPatch("{id:Guid}/password/change")]
        public async Task<IActionResult> ChangePassword(
            Guid id,
            [FromBody] ChangePasswordRequest request
        )
        {
            return HandleResponse(await _userService.ChangePasswordAsync(id, request.NewPassword));
        }

        [HttpPatch("{id:Guid}/password/reset")]
        [Authorize(Policy = RoleAccess.ADMIN)]
        public async Task<IActionResult> ResetPassword(Guid id)
        {
            return HandleResponse(await _userService.ResetPasswordAsync(id));
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Policy = RoleAccess.ADMIN)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            return HandleResponse(await _userService.DeleteUserAsync(id));
        }

        [HttpPatch("{id:Guid}/restore")]
        [Authorize(Policy = RoleAccess.ADMIN)]
        public async Task<IActionResult> RestoreUser(Guid id)
        {
            return HandleResponse(await _userService.RestoreUserAsync(id));
        }
    }
}
