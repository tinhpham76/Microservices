using auth_server.Authorization;
using auth_services.Constants;
using auth_services.RequestModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace auth_server.Controllers
{
    public class IdentityResourcesController : BaseController
    {
        private readonly ConfigurationDbContext _configurationDbContext;
        public IdentityResourcesController(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        [HttpPost]
        [ClaimRequirement(PermissionCode.ADMIN_API_CREATE)]
        public async Task<IActionResult> PostIdentityResource([FromBody] IdentityResourceRequestModel request)
        {
            var identityResource = await _configurationDbContext.IdentityResources.FirstOrDefaultAsync(x => x.Name == request.Name);
            if (identityResource != null)
            {
                return BadRequest();
            }
            var identityResourceRequest = new IdentityResource()
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
                Description = request.Description,
                Enabled = request.Enabled,
                ShowInDiscoveryDocument = request.ShowInDiscoveryDocument,
                Required = request.Required,
                Emphasize = request.Emphasize,
                UserClaims = request.UserClaims
            };
            _configurationDbContext.IdentityResources.Add(identityResourceRequest.ToEntity());
            var result = await _configurationDbContext.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }

        //Delete Identity Resource
        [HttpDelete("{identityResourceName}")]
        [ClaimRequirement(PermissionCode.ADMIN_API_DELETE)]
        public async Task<IActionResult> DeleteIdentityResource(string identityResourceName)
        {
            var identityResource = await _configurationDbContext.IdentityResources.FirstOrDefaultAsync(x => x.Name == identityResourceName);
            if (identityResource == null)
                return NotFound();
            _configurationDbContext.IdentityResources.Remove(identityResource);
            var result = await _configurationDbContext.SaveChangesAsync();
            if (result > 0)
                return Ok();
            return BadRequest();
        }
    }
}
