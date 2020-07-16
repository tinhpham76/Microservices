using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSO.Services.RequestModels;

namespace SSO.Backend.Controllers
{
    public class ClientsController : BaseController
    {
        private readonly ConfigurationDbContext _configurationDbContext;
        private readonly IClientStore _clientStore;
        public ClientsController(ConfigurationDbContext configurationDbContext, IClientStore clientStore)
        {
            _configurationDbContext = configurationDbContext;
            _clientStore = clientStore;
        }

        //Post client
        [HttpPost]
        public async Task<IActionResult> PostClient([FromBody] ClientRequest request)
        {
            var client = await _configurationDbContext.Clients.FirstOrDefaultAsync(x => x.ClientName == request.ClientName);
            if (client != null)
            {
                return BadRequest($"Client {request.ClientName} already exist!");
            }
            var clientRequest = new Client()
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = request.ClientName,
                Description = request.Description,
                ClientUri = request.ClientUri,
                LogoUri = request.LogoUri,
                AllowedGrantTypes = request.ClientType.Equals("web_app_authorization_code") ? GrantTypes.Code
                    : request.ClientType.Equals("spa") ? GrantTypes.Code
                    : request.ClientType.Equals("native") ? GrantTypes.Code
                    : request.ClientType.Equals("web_app_hybrid") ? GrantTypes.Hybrid
                    : request.ClientType.Equals("server") ? GrantTypes.ClientCredentials
                    : request.ClientType.Equals("device") ? GrantTypes.DeviceFlow : GrantTypes.Implicit,

            };
            _configurationDbContext.Add(clientRequest.ToEntity());
            var result = await _configurationDbContext.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }

        // Delete Client
        [HttpDelete("{clientId}")]
        public async Task<IActionResult> DeleteClient(string clientId)
        {
            var client = await _configurationDbContext.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if(client == null)
            {
                return NotFound();
            }
            _configurationDbContext.Clients.Remove(client);
            var result = await _configurationDbContext.SaveChangesAsync();
            if(result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}