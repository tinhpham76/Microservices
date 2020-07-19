using auth_server.Data;
using auth_services;
using auth_services.RequestModel;
using auth_services.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace auth_server.Controllers
{
    public class RolesController : BaseController
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;           
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound();
            var roleViewModel = new RoleViewModel()
            {
                Id = role.Id,
                Name = role.Name,
                NormalizedName = role.NormalizedName
            };
            return Ok(roleViewModel);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetRolesPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _roleManager.Roles;
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.Name.Contains(filter));
            }
            var totalReconds = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new RoleViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    NormalizedName = x.NormalizedName
                }).ToListAsync();
            var pagination = new Pagination<RoleViewModel>
            {
                TotalRecords = totalReconds,
                Items = items
            };
            return Ok(pagination);
        }

        [HttpPost]
        public async Task<IActionResult> PostRole([FromBody] RoleRequestModel request)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name == request.Name);
            if (role != null)
                return BadRequest($"Role name {request.Name} already exist!");
            var roleRequest = new IdentityRole()
            {
                Id = request.Id,
                Name = request.Name,
                NormalizedName = request.NormalizedName.ToUpper()
            };
            var result = await _roleManager.CreateAsync(roleRequest);
            if (result.Succeeded)
                return Ok();
            return BadRequest();
        }

        [HttpPut("{roleId}")]
        public async Task<IActionResult> PutRole(string roleId, [FromBody] RoleRequestModel request)
        {
            if (roleId != request.Id)
                return BadRequest("Role id not match");

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound();

            role.Name = request.Name;
            role.NormalizedName = request.NormalizedName.ToUpper();           
           
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
                return Ok();
            return BadRequest();
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound();

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
                return Ok();
            return BadRequest();
        }

        [HttpGet("{roleId}/claims")]
        public async Task<IActionResult> GetRoleClaims(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            if (roleClaims == null)
            {
                return NotFound();
            }
            var roleClaimViewModels = roleClaims.Select(x => new RoleClaimViewModels()
            {
                Type = x.Type,
                Value = x.Value
            }).ToList();
           
            return Ok(roleClaimViewModels);
        }

        [HttpPost("{roleId}/claims")]
        public async Task<IActionResult> PostRoleClaims(string roleId, [FromBody] RoleClaimRequestModels<RoleClaimRequestModel> request)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            if (roleClaims.Count == 0)
            {
                foreach (var requestClaim in request.Claims)
                {
                    var claim = new Claim(requestClaim.Type, requestClaim.Value);
                    await _roleManager.AddClaimAsync(role, claim);
                }
            }
            else if (request.Claims.Count == 0)
            {
                foreach (var roleClaim in roleClaims)
                {
                    await _roleManager.RemoveClaimAsync(role, roleClaim);
                }
            }
            else
            {
                foreach (var roleClaim in roleClaims)
                {
                    await _roleManager.RemoveClaimAsync(role, roleClaim);
                }
                foreach (var requestClaim in request.Claims)
                {
                    var claim = new Claim(requestClaim.Type, requestClaim.Value);
                    await _roleManager.AddClaimAsync(role, claim);
                }
            }
            var roleClaimViewModel = await _roleManager.GetClaimsAsync(role);
            return Ok(roleClaimViewModel);

        }
    }
}
