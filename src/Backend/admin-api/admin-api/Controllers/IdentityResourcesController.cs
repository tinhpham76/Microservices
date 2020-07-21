using admin_api.Data;
using admin_api.Data.Entities;
using admin_api.Services;
using admin_services;
using admin_services.RequestModels;
using admin_services.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin_api.Controllers
{
    public class IdentityResourcesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IIdentityResourceApiClient _identityResourceApiClient;
        public IdentityResourcesController(ApplicationDbContext context, IIdentityResourceApiClient identityResourceApiClient)
        {
            _context = context;
            _identityResourceApiClient = identityResourceApiClient;
        }

        #region Identity Resource
        // Find identity resource with name or display name
        [HttpGet("filter")]
        public async Task<IActionResult> GetIdentityResourcesPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.IdentityResources.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.Name.Contains(filter) || x.DisplayName.Contains(filter));

            }
            var totalReconds = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new IdentityResourceQuickViewModels()
                {
                    Name = x.Name,
                    DisplayName = x.DisplayName,
                    Description = x.Description
                }).ToListAsync();

            var pagination = new Pagination<IdentityResourceQuickViewModels>
            {
                Items = items,
                TotalRecords = totalReconds
            };
            return Ok(pagination);
        }

        [HttpPost]
        public async Task<IActionResult> PostIdentityResource([FromBody] IdentityResourceRequestModel request)
        {
            var apiScope = await _context.ApiScopes.Select(x => x.Name.ToString()).ToListAsync();
            if (apiScope.Contains(request.Name))
            {
                return BadRequest();
            }
            var result = await _identityResourceApiClient.PostIdentityResource(request);
            if (result == true)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("{identityResourceName}")]
        public async Task<IActionResult> DeleteApiResource(string identityResourceName)
        {
            var temp = new List<string>() { "address", "phone", "email", "profile", "openid" };
            if (temp.Contains(identityResourceName))
            {
                return BadRequest();
            }
            var result = await _identityResourceApiClient.DeleteIdentityResource(identityResourceName);
            if (result == true)
            {
                return Ok();
            }
            return BadRequest();
        }

        //Get detail info identity resource
        [HttpGet("{identityResourceName}")]
        public async Task<IActionResult> GetIdentityResource(string identityResourceName)
        {
            var identityResource = await _context.IdentityResources.FirstOrDefaultAsync(x => x.Name == identityResourceName);
            if (identityResource == null)
                return NotFound();
            var claims = await _context.IdentityResourceClaims
                .Where(x => x.IdentityResourceId == identityResource.Id)
                .Select(x => x.Type.ToString()).ToListAsync();
            var identityResourceViewModel = new IdentityResourceViewModel()
            {
                Name = identityResource.Name,
                DisplayName = identityResource.DisplayName,
                Description = identityResource.Description,
                Enabled = identityResource.Enabled,
                ShowInDiscoveryDocument = identityResource.ShowInDiscoveryDocument,
                Required = identityResource.Required,
                Emphasize = identityResource.Emphasize,
                UserClaims = claims
            };
            return Ok(identityResourceViewModel);
        }

        [HttpPut("{identityResourceName}")]
        public async Task<IActionResult> PutApiResource(string identityResourceName, [FromBody] IdentityResourceRequestModel request)
        {
            var identityResource = await _context.IdentityResources.FirstOrDefaultAsync(x => x.Name == identityResourceName);
            if (identityResource == null)
            {
                return NotFound();
            }
            identityResource.DisplayName = request.DisplayName;
            identityResource.Description = request.Description;
            identityResource.Enabled = request.Enabled;
            identityResource.ShowInDiscoveryDocument = request.ShowInDiscoveryDocument;
            identityResource.Required = request.Required;
            identityResource.Emphasize = request.Emphasize;
            identityResource.Updated = DateTime.UtcNow;

            // Claim
            var claims = await _context.IdentityResourceClaims
                  .Where(x => x.IdentityResourceId == identityResource.Id)
                  .Select(x => x.Type.ToString()).ToListAsync();
            foreach (var claim in claims)
            {
                if (!(request.UserClaims.Contains(claim)))
                {
                    var removeClaim = await _context.IdentityResourceClaims.FirstOrDefaultAsync(x => x.Type == claim);
                    _context.IdentityResourceClaims.Remove(removeClaim);
                }
            }

            foreach (var requestClaim in request.UserClaims)
            {
                if (!claims.Contains(requestClaim))
                {
                    var addClaim = new IdentityResourceClaim()
                    {
                        Type = requestClaim,
                        IdentityResourceId = identityResource.Id
                    };
                    _context.IdentityResourceClaims.Add(addClaim);
                }
            }
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }
        #endregion

        #region Identity Resource Property      
        //Get identity resource properties
        [HttpGet("{identityResourceName}/properties")]
        public async Task<IActionResult> GetIdentityResourceProperties(string identityResourceName)
        {
            var identityResource = await _context.IdentityResources.FirstOrDefaultAsync(x => x.Name == identityResourceName);
            if (identityResource == null)
            {
                return NotFound();
            }
            var query = _context.IdentityResourceProperties.Where(x => x.IdentityResourceId.Equals(identityResource.Id));
            var identityResourceProperties = await query.Select(x => new IdentityResourcePropertyViewModels()
            {
                Id = x.Id,
                Key = x.Key,
                Value = x.Value
            }).ToListAsync();
            return Ok(identityResourceProperties);
        }

        //Post identity resource property
        [HttpPost("{identityResourceName}/properties")]
        public async Task<IActionResult> PostApiResourceProperty(string identityResourceName, [FromBody] IdentityResourcePropertyRequestModel request)
        {
            var identityResource = await _context.IdentityResources.FirstOrDefaultAsync(x => x.Name == identityResourceName);
            if (identityResource == null)
            {
                return BadRequest();
            }
            var identityProperty = await _context.IdentityResourceProperties.FirstOrDefaultAsync(x => x.Key == request.Key && x.IdentityResourceId == identityResource.Id);
            if (identityProperty != null)
            {
                return BadRequest();
            }
            var identityPropertyRequest = new IdentityResourceProperty()
            {
                Key = request.Key,
                Value = request.Value,
                IdentityResourceId = identityResource.Id
            };
            _context.IdentityResourceProperties.Add(identityPropertyRequest);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }

        //Delete api resource property
        [HttpDelete("{identityResourceName}/properties/{propertyKey}")]
        public async Task<IActionResult> DeleteIdentityResourceProperty(string identityResourceName, string propertyKey)
        {
            var identityResource = await _context.IdentityResources.FirstOrDefaultAsync(x => x.Name == identityResourceName);
            if (identityResource == null)
                return NotFound();
            var identityResourceProperty = await _context.IdentityResourceProperties.FirstOrDefaultAsync(x => x.IdentityResourceId == identityResource.Id && x.Key == propertyKey);
            if (identityResourceProperty == null)
                return NotFound();
            _context.IdentityResourceProperties.Remove(identityResourceProperty);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }
        #endregion
    }
}
