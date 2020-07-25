using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using user_api.Services;
using user_services.RequestModels;
using user_services.ViewModels;

namespace user_api.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUserApiClient _userApiClient;

        public UsersController(IUserApiClient userApiClient)
        {
            _userApiClient = userApiClient;
        }

        // Post user
        [HttpPost]
        public async Task<ActionResult> PostUser([FromBody] UserRequestModel request)
        {
            var result = await _userApiClient.PostUser(request);
            if (result == true)
            {
                return Ok();
            }
            return BadRequest();
        }

        // Get users
        [HttpGet("filter")]
        public async Task<IActionResult> GetUsersPaging(string filter, int pageIndex, int pageSize)
        {
            var users = await _userApiClient.GetUsersPaging(filter, pageIndex, pageSize);
            return Ok(users);
        }

        // Get user detail
        [HttpGet("{userId}")]
        public async Task<ActionResult> GetUserDetail(string userId)
        {
            var user = await _userApiClient.GetById(userId);
            return Ok(user);
        }

        // Put User
        [HttpPut("{userId}")]
        public async Task<ActionResult> PutUser(string userId, [FromBody] UserRequestModel request)
        {
            var result = await _userApiClient.PutUser(userId, request);
            if (result == true)
            {
                return Ok();
            }
            return BadRequest();
        }

        // Reset password
        [HttpPut("{userId}/reset-password")]
        public async Task<ActionResult> PutResetPassword(string userId)
        {
            var result = await _userApiClient.PutResetPassword(userId);
            if (result == true)
            {
                return Ok();
            }
            return BadRequest();
        }

        // Change password
        [HttpPut("{userId}/change-password")]
        public async Task<ActionResult> PutUserPassword(string userId, [FromBody] UserPasswordRequestModel request)
        {
            var result = await _userApiClient.PutUserPassword(userId, request);
            if (result == true)
            {
                return Ok();
            }
            return BadRequest();
        }

        // Delete user
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _userApiClient.DeleteUser(userId);
            if (result == true)
            {
                return Ok();
            }
            return BadRequest();
        }

        // Get user roles
        [HttpGet("{userId}/userRoles")]
        public async Task<IActionResult> GetUserDetailWithRoles(string userId)
        {
            var userRoles = await _userApiClient.GetUserDetailWithRoles(userId);
            return Ok(userRoles);
        }

        // Put user roles
        [HttpPut("{userId}/userRoles")]
        public async Task<IActionResult> PutUserDetailWithRoles(string userId, [FromBody] UserRoleRequestModel request)
        {

            var result = await _userApiClient.PutUserDetailWithRoles(userId, request);
            if (result == true)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
