using auth_services.RequestModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace auth_server.Controllers
{
    public class ApiResourcesController : BaseController
    {
        private readonly ConfigurationDbContext _configurationDbContext;
        public ApiResourcesController(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        //Post basic infor api resource
        [HttpPost]
        public async Task<IActionResult> PostApiResource([FromBody] ApiResourceRequestModel request)
        {
            var apiResource = await _configurationDbContext.ApiResources.FirstOrDefaultAsync(x => x.Name == request.Name);
            if (apiResource != null)
                return BadRequest();
            var apiResourceRequest = new ApiResource()
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
                Description = request.Description,
                Enabled = request.Enabled,
                AllowedAccessTokenSigningAlgorithms = { request.AllowedAccessTokenSigningAlgorithms },
                ShowInDiscoveryDocument = request.ShowInDiscoveryDocument,
                UserClaims = request.UserClaims,
                Scopes = request.Scopes
            };
            _configurationDbContext.ApiResources.Add(apiResourceRequest.ToEntity());
            var result = await _configurationDbContext.SaveChangesAsync();
            if (result > 0)
                return Ok();
            return BadRequest();
        }

        //Delete api resource
        [HttpDelete("{apiResourceName}")]
        public async Task<IActionResult> DeleteApiResource(string apiResourceName)
        {
            var apiResource = await _configurationDbContext.ApiResources.FirstOrDefaultAsync(x => x.Name == apiResourceName);
            if (apiResource == null)
                return NotFound();
            _configurationDbContext.ApiResources.Remove(apiResource);
            var result = await _configurationDbContext.SaveChangesAsync();
            if (result > 0)
                return Ok();
            return BadRequest();
        }
    }
}
