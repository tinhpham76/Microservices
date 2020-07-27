using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using user_api.Authorization;
using user_api.Services;
using user_services.Constants;
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

        // Get role detail
        [HttpGet("{roleId}")]
        [ClaimRequirement(PermissionCode.USER_API_VIEW)]
        public async Task<IActionResult> GetRole(string roleId)
        {
            var roles = await _roleApiClient.GetRole(roleId);
            return Ok(roles);
        }

        // Get roles
        [HttpGet("filter")]
        [ClaimRequirement(PermissionCode.USER_API_VIEW)]
        public async Task<IActionResult> GetRolesPaging(string filter, int pageIndex, int pageSize)
        {
            var roles = await _roleApiClient.GetRolesPaging(filter, pageIndex, pageSize);
            return Ok(roles);
        }

        // Post role
        [HttpPost]
        [ClaimRequirement(PermissionCode.USER_API_CREATE)]
        public async Task<IActionResult> PostRole([FromBody] RoleRequestModel request)
        {
            var result = await _roleApiClient.PostRole(request);
            if (result == true)
                return Ok(result);
            return BadRequest(result);
        }

        // Put role
        [HttpPut("{roleId}")]
        [ClaimRequirement(PermissionCode.USER_API_UPDATE)]
        public async Task<IActionResult> PutRole(string roleId, [FromBody] RoleRequestModel request)
        {
            var result = await _roleApiClient.PutRole(roleId, request);
            if (result == true)
                return Ok(result);
            return BadRequest(result);
        }

        // Delete role
        [HttpDelete("{roleId}")]
        [ClaimRequirement(PermissionCode.USER_API_DELETE)]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var result = await _roleApiClient.DeleteRole(roleId);
            if (result == true)
                return Ok(result);
            return BadRequest(result);
        }

        // Get roles with claims
        [HttpGet("{roleId}/claims/filter")]
        [ClaimRequirement(PermissionCode.USER_API_VIEW)]
        public async Task<IActionResult> GetRoleClaims(string roleId, string filter, int pageIndex, int pageSize)
        {
            var roleClaims = await _roleApiClient.GetRoleClaims(roleId, filter, pageIndex, pageSize);
            return Ok(roleClaims);
        }

        // Post claims to role
        [HttpPost("{roleId}/claims")]
        [ClaimRequirement(PermissionCode.USER_API_CREATE)]
        public async Task<IActionResult> PostRoleClaims(string roleId, [FromBody] RoleClaimRequestModel request)
        {
            var result = await _roleApiClient.PostRoleClaims(roleId, request);
            if (result == true)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
