﻿using auth_server.Data;
using auth_services;
using auth_services.Constants;
using auth_services.RequestModel;
using auth_services.ViewModel;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace auth_server.Controllers
{
    public class RolesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ConfigurationDbContext _configurationDbContext;
        public RolesController(RoleManager<IdentityRole> roleManager, ConfigurationDbContext configurationDbContext, ApplicationDbContext context)
        {
            _context = context;
            _configurationDbContext = configurationDbContext;
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


        [HttpGet("{roleId}/claims/filter")]
        public async Task<IActionResult> GetRoleClaims(string roleId, string filter, int pageIndex, int pageSize)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }
            var claimTypes = await _configurationDbContext.ApiResources.Select(x => x.Name.ToString()).ToListAsync();
            var claims = new List<ApiRoleViewModel>();
            foreach (var claimType in claimTypes)
            {
                var claimValue = await _context.RoleClaims.Where(x => x.ClaimType == claimType && x.RoleId == roleId).Select(v => v.ClaimValue.ToString()).ToListAsync();
                var claim = new ApiRoleViewModel()
                {
                    Type = claimType,
                    View = claimValue.Contains(SystemConstants.View.Type) ? true : false,
                    Create = claimValue.Contains(SystemConstants.Create.Type) ? true : false,
                    Update = claimValue.Contains(SystemConstants.Update.Type) ? true : false,
                    Delete = claimValue.Contains(SystemConstants.Delete.Type) ? true : false,
                };
                claims.Add(claim);
            }
            var query = claims.AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.Type.Contains(filter));
            }
            var totalReconds = query.Count();
            var items = query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ApiRoleViewModel()
                {
                    Type = x.Type,
                    View =x.View,
                    Create =x.Create,
                    Update = x.Update,
                    Delete = x.Delete
                }).ToList();
            var pagination = new Pagination<ApiRoleViewModel>
            {
                TotalRecords = totalReconds,
                Items = items
            };
            return Ok(pagination);
         
        }

        [HttpPost("{roleId}/claims")]
        public async Task<IActionResult> PostRoleClaims(string roleId, [FromBody] RoleClaimRequestModel request)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }

            var apiResource = await _configurationDbContext.ApiResources.FirstOrDefaultAsync(x => x.Name == request.Type);
            if (apiResource == null)
            {
                return BadRequest();
            }
            else
            {
                if (request.View == false)
                {
                    await _roleManager.RemoveClaimAsync(role, new Claim(request.Type, SystemConstants.View.Type));
                }
                else if (request.View == true)
                {
                    await _roleManager.RemoveClaimAsync(role, new Claim(request.Type, SystemConstants.View.Type));
                    await _roleManager.AddClaimAsync(role, new Claim(request.Type, SystemConstants.View.Type));
                }
                if (request.Create == false)
                {
                    await _roleManager.RemoveClaimAsync(role, new Claim(request.Type, SystemConstants.Create.Type));
                }
                else if (request.Create == true)
                {
                    await _roleManager.RemoveClaimAsync(role, new Claim(request.Type, SystemConstants.Create.Type));
                    await _roleManager.AddClaimAsync(role, new Claim(request.Type, SystemConstants.Create.Type));
                }
                if (request.Update == false)
                {
                    await _roleManager.RemoveClaimAsync(role, new Claim(request.Type, SystemConstants.Update.Type));
                }
                else if (request.Update == true)
                {
                    await _roleManager.RemoveClaimAsync(role, new Claim(request.Type, SystemConstants.Update.Type));
                    await _roleManager.AddClaimAsync(role, new Claim(request.Type, SystemConstants.Update.Type));
                }
                if (request.Delete == false)
                {
                    await _roleManager.RemoveClaimAsync(role, new Claim(request.Type, SystemConstants.Delete.Type));
                }
                else if (request.Delete == true)
                {
                    await _roleManager.RemoveClaimAsync(role, new Claim(request.Type, SystemConstants.Delete.Type));
                    await _roleManager.AddClaimAsync(role, new Claim(request.Type, SystemConstants.Delete.Type));
                }
            }

            var permission = await _roleManager.GetClaimsAsync(role);
            return Ok(permission);

        }
    }
}
