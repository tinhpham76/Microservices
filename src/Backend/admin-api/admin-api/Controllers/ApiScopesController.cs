using admin_api.Authorization;
using admin_api.Data;
using admin_api.Data.Entities;
using admin_api.Services;
using admin_services;
using admin_services.Constants;
using admin_services.RequestModels;
using admin_services.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace admin_api.Controllers
{
    public class ApiScopesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IApiScopeApiClient _apiScopeApiClient;
        public ApiScopesController(ApplicationDbContext context, IApiScopeApiClient apiScopeApiClient)
        {
            _context = context;
            _apiScopeApiClient = apiScopeApiClient;
        }

        #region Api Scope
        // Find api resource with name or display name
        [HttpGet("filter")]
        [ClaimRequirement(PermissionCode.ADMIN_API_VIEW)]
        public async Task<IActionResult> GetApiScopesPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.ApiScopes.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.Name.Contains(filter) || x.DisplayName.Contains(filter));

            }
            var totalReconds = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ApiScopeQuickViewModels()
                {
                    Name = x.Name,
                    DisplayName = x.DisplayName,
                    Description = x.Description
                }).ToListAsync();

            var pagination = new Pagination<ApiScopeQuickViewModels>
            {
                Items = items,
                TotalRecords = totalReconds
            };
            return Ok(pagination);
        }

        [HttpPost]
        [ClaimRequirement(PermissionCode.ADMIN_API_CREATE)]
        public async Task<IActionResult> PostApiScope([FromBody] ApiScopeRequestModel request)
        {

            var apiScope = await _context.IdentityResources.Select(x => x.Name.ToString()).ToListAsync();
            if (apiScope.Contains(request.Name))
            {
                return BadRequest();
            }
            var result = await _apiScopeApiClient.PostApiScope(request);
            if (result == true)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("{apiScopeName}")]
        [ClaimRequirement(PermissionCode.ADMIN_API_DELETE)]
        public async Task<IActionResult> DeleteApiScope(string apiScopeName)
        {
            var result = await _apiScopeApiClient.DeleteApiScope(apiScopeName);
            if (result == true)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("{apiScopeName}")]
        [ClaimRequirement(PermissionCode.ADMIN_API_VIEW)]
        public async Task<IActionResult> GetDetailApiScope(string apiScopeName)
        {
            var apiScope = await _context.ApiScopes.FirstOrDefaultAsync(x => x.Name == apiScopeName);
            if (apiScope == null)
            {
                return NotFound();
            }
            var claims = await _context.ApiScopeClaims
                 .Where(x => x.ScopeId == apiScope.Id)
                 .Select(x => x.Type.ToString()).ToListAsync();

            var apiScopeViewModel = new ApiScopeViewModel()
            {
                Name = apiScope.Name,
                DisplayName = apiScope.DisplayName,
                Description = apiScope.Description,
                Emphasize = apiScope.Emphasize,
                Enabled = apiScope.Enabled,
                ShowInDiscoveryDocument = apiScope.ShowInDiscoveryDocument,
                UserClaims = claims,
                Required = apiScope.Required
            };
            return Ok(apiScopeViewModel);
        }

        [HttpPut("{apiScopeName}")]
        [ClaimRequirement(PermissionCode.ADMIN_API_UPDATE)]
        public async Task<IActionResult> PutApiScope(string apiScopeName, [FromBody] ApiScopeRequestModel request)
        {
            var apiScope = await _context.ApiScopes.FirstOrDefaultAsync(x => x.Name == apiScopeName);
            if (apiScope == null)
            {
                return NotFound();
            }
            apiScope.DisplayName = request.DisplayName;
            apiScope.Description = request.Description;
            apiScope.Enabled = request.Enabled;
            apiScope.ShowInDiscoveryDocument = request.ShowInDiscoveryDocument;
            apiScope.Required = request.Required;
            apiScope.Emphasize = request.Emphasize;
            // Claim
            var claims = await _context.ApiScopeClaims
                  .Where(x => x.ScopeId == apiScope.Id)
                  .Select(x => x.Type.ToString()).ToListAsync();
            foreach (var claim in claims)
            {
                if (!(request.UserClaims.Contains(claim)))
                {
                    var removeClaim = await _context.ApiScopeClaims.FirstOrDefaultAsync(x => x.Type == claim && x.ScopeId == apiScope.Id);
                    _context.ApiScopeClaims.Remove(removeClaim);
                }
            }

            foreach (var requestClaim in request.UserClaims)
            {
                if (!claims.Contains(requestClaim))
                {
                    var addClaim = new ApiScopeClaim()
                    {
                        Type = requestClaim,
                        ScopeId = apiScope.Id
                    };
                    _context.ApiScopeClaims.Add(addClaim);
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

        #region Api Scope Property
        //Get api scope properties
        [HttpGet("{apiScopeName}/properties")]
        [ClaimRequirement(PermissionCode.ADMIN_API_VIEW)]
        public async Task<IActionResult> GetApiScopeProperties(string apiScopeName)
        {
            var apiScope = await _context.ApiScopes.FirstOrDefaultAsync(x => x.Name == apiScopeName);
            if (apiScope == null)
            {
                return NotFound();
            }
            var query = _context.ApiScopeProperties.Where(x => x.ScopeId.Equals(apiScope.Id));
            var apiScopeProperties = await query.Select(x => new ApiScopePropertyViewModels()
            {
                Id = x.Id,
                Key = x.Key,
                Value = x.Value
            }).ToListAsync();
            return Ok(apiScopeProperties);
        }

        //Post api scope property
        [HttpPost("{apiScopeName}/properties")]
        [ClaimRequirement(PermissionCode.ADMIN_API_CREATE)]
        public async Task<IActionResult> PostApiScopeProperty(string apiScopeName, [FromBody] ApiScopePropertyRequestModel request)
        {
            var apiScope = await _context.ApiScopes.FirstOrDefaultAsync(x => x.Name == apiScopeName);
            if (apiScope == null)
            {
                return BadRequest();
            }
            var apiProperty = await _context.ApiScopeProperties.FirstOrDefaultAsync(x => x.Key == request.Key && x.ScopeId == apiScope.Id);
            if (apiProperty != null)
            {
                return BadRequest();
            }
            var apiPropertyRequest = new ApiScopeProperty()
            {
                Key = request.Key,
                Value = request.Value,
                ScopeId = apiScope.Id
            };
            _context.ApiScopeProperties.Add(apiPropertyRequest);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }

        //Delete api scope property
        [HttpDelete("{apiScopeName}/properties/{propertyKey}")]
        [ClaimRequirement(PermissionCode.ADMIN_API_DELETE)]
        public async Task<IActionResult> DeleteApiScopeProperty(string apiScopeName, string propertyKey)
        {
            var apiScope = await _context.ApiScopes.FirstOrDefaultAsync(x => x.Name == apiScopeName);
            if (apiScope == null)
                return NotFound();
            var apiScopeProperty = await _context.ApiScopeProperties.FirstOrDefaultAsync(x => x.ScopeId == apiScope.Id && x.Key == propertyKey);
            if (apiScopeProperty == null)
                return NotFound();
            _context.ApiScopeProperties.Remove(apiScopeProperty);
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

