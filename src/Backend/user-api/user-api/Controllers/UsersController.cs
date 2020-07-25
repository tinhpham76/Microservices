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

        [HttpGet("filter")]
        public async Task<IActionResult> GetUsersPaging(string filter, int pageIndex, int pageSize)
        {
            var users = await _userApiClient.GetUsersPaging(filter, pageIndex, pageSize);
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult> GetUserDetail(string userId)
        {
            var user = await _userApiClient.GetById(userId);
            return Ok(user);
        }

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

        [HttpGet("{userId}/userRoles")]
        public async Task<IActionResult> GetUserDetailWithRoles(string userId)
        {
            var userRoles = await _userApiClient.GetUserDetailWithRoles(userId);
            return Ok(userRoles);
        }

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
