using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using user_api.Services;
using user_services.RequestModels;

namespace user_api.Controllers
{
    public class RolesController : BaseController
    {
        private readonly IRoleApiClient _roleApiClient;

        public RolesController(IRoleApiClient roleApiClient)
        {
            _roleApiClient = roleApiClient;
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRole(string roleId)
        {
            var roles = await _roleApiClient.GetRole(roleId);
            return Ok(roles);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetRolesPaging(string filter, int pageIndex, int pageSize)
        {
            var roles = await _roleApiClient.GetRolesPaging(filter, pageIndex, pageSize);
            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> PostRole([FromBody] RoleRequestModel request)
        {
            var result = await _roleApiClient.PostRole(request);
            if (result == true)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("{roleId}")]
        public async Task<IActionResult> PutRole(string roleId, [FromBody] RoleRequestModel request)
        {
            var result = await _roleApiClient.PutRole(roleId, request);
            if (result == true)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var result = await _roleApiClient.DeleteRole(roleId);
            if (result == true)
                return Ok(result);
            return BadRequest(result);
        }



        [HttpGet("{roleId}/claims/filter")]
        public async Task<IActionResult> GetRoleClaims(string roleId, string filter, int pageIndex, int pageSize)
        {
            var roleClaims = await _roleApiClient.GetRoleClaims(roleId, filter, pageIndex, pageSize);
            return Ok(roleClaims);
        }

        [HttpPost("{roleId}/claims")]
        public async Task<IActionResult> PostRoleClaims(string roleId, [FromBody] RoleClaimRequestModel request)
        {
            var result = await _roleApiClient.PostRoleClaims(roleId, request);
            if (result == true)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
