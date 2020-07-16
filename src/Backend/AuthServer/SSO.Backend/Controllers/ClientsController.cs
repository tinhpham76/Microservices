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

        /// <summary>
        /// Get all client
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> PutClient([FromBody]Client client)
        {
            var check = await _clientStore.FindClientByIdAsync(client.ClientId);
            if (check == null)
            {
                return BadRequest();
            }
            check.Description = client.Description;
            var clients = _configurationDbContext.Clients.Update(check.ToEntity());
            var result = await _configurationDbContext.SaveChangesAsync();
            if (result < 0)
            {
                return BadRequest(result);

            }
            return Ok(clients);
        }
        [HttpGet("clientId")]
        public async Task<IActionResult> GetClient(string ClientId)
        {
            var check =await _clientStore.FindClientByIdAsync(ClientId);           
            
            return Ok(check);
        }
    }
}
