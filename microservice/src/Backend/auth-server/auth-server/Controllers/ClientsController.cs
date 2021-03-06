﻿using auth_server.Authorization;
using auth_services.Constants;
using auth_services.RequestModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace auth_server.Controllers
{
    public class ClientsController : BaseController
    {
        private readonly ConfigurationDbContext _configurationDbContext;
        public ClientsController(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        //Post client
        [HttpPost]
        [ClaimRequirement(PermissionCode.ADMIN_API_CREATE)]
        public async Task<IActionResult> PostClient([FromBody] ClientRequestModel request)
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
            _configurationDbContext.Clients.Add(clientRequest.ToEntity());
            var result = await _configurationDbContext.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }

        // Delete Client
        [HttpDelete("{clientId}")]
        [ClaimRequirement(PermissionCode.ADMIN_API_DELETE)]
        public async Task<IActionResult> DeleteClient(string clientId)
        {
            var client = await _configurationDbContext.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
            {
                return NotFound();
            }
            _configurationDbContext.Clients.Remove(client);
            var result = await _configurationDbContext.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
