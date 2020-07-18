using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetUsersPaging(string filter, int pageIndex, int pageSize)
        {
            var users = await _userApiClient.GetUsersPaging(filter, pageIndex, pageSize);
            return Ok(users);
        }

        [HttpGet("{id}")]        
        public async Task<ActionResult> GetUserDetail(string id)
        {
            var user = await _userApiClient.GetById(id);
            return Ok(user);
        }

        [HttpPut("{id}")]       
        public async Task<ActionResult> PutUser(string id, [FromBody] UserRequestModel request)
        {
            var result = await _userApiClient.PutUser(id, request);
            if (result == true)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("{id}/reset-password")]
        public async Task<ActionResult> PutResetPassword(string id)
        {
            var result = await _userApiClient.PutResetPassword(id);
            if (result == true)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("{id}/change-password")]
        public async Task<ActionResult> PutUserPassword(string id, [FromBody] UserPasswordRequestModel request)
        {
            var result = await _userApiClient.PutUserPassword(id, request);
            if (result == true)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{id}")]        
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userApiClient.DeleteUser(id);
            if (result == true)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("{id}/roles")]        
        public async Task<ActionResult> GetUserRoles(string id)
        {
            var userRoles = await _userApiClient.GetUserRoles(id);
            return Ok(userRoles);
        }

        [HttpPost("{userId}/roles")]
        public async Task<ActionResult> PostRolesToUser(string userId, [FromBody] RoleAssignRequestModel request)
        {
            var result = await _userApiClient.PostRolesToUser(userId, request);
            if (result == true)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPut("{userId}/roles")]
        public async Task<ActionResult> RemoveRolesFromUser(string userId, [FromBody] RoleAssignRequestModel request)
        {
            var result = await _userApiClient.RemoveRolesFromUser(userId, request);
            if (result == true)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
