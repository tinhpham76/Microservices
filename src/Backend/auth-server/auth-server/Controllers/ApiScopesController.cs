using auth_services.RequestModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace auth_server.Controllers
{
    public class ApiScopesController : BaseController
    {
        private readonly ConfigurationDbContext _configurationDbContext;
        public ApiScopesController(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        //Post basic infor api scope
        [HttpPost]
        public async Task<IActionResult> PostApiScope([FromBody] ApiScopeRequestModel request)
        {
            var apiScope = await _configurationDbContext.ApiScopes.FirstOrDefaultAsync(x => x.Name == request.Name);
            if (apiScope != null)
                return BadRequest($"Api Resource name {request.Name} already exist");
            var apiApiScopeRequest = new ApiScope()
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
                Description = request.Description,
                Enabled = request.Enabled,
                ShowInDiscoveryDocument = request.ShowInDiscoveryDocument,
                Emphasize = request.Emphasize,
                Required = request.Required,
                UserClaims = request.UserClaims
            };
            _configurationDbContext.ApiScopes.Add(apiApiScopeRequest.ToEntity());
            var result = await _configurationDbContext.SaveChangesAsync();
            if (result > 0)
                return Ok();
            return BadRequest();
        }

        //Delete api scope
        [HttpDelete("{apiScopeName}")]
        public async Task<IActionResult> DeleteApiScope(string apiScopeName)
        {
            var apiScope = await _configurationDbContext.ApiScopes.FirstOrDefaultAsync(x => x.Name == apiScopeName);
            if (apiScope == null)
                return NotFound();
            _configurationDbContext.ApiScopes.Remove(apiScope);
            var result = await _configurationDbContext.SaveChangesAsync();
            if (result > 0)
                return Ok();
            return BadRequest();
        }
    }
}
