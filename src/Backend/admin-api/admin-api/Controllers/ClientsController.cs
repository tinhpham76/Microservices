using admin_api.Data;
using admin_api.Data.Entities;
using admin_api.Services;
using admin_services;
using admin_services.RequestModels;
using admin_services.ViewModels;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin_api.Controllers
{
    public class ClientsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IClientApiClient _clientApiClient;
        public ClientsController(ApplicationDbContext context, IClientApiClient clientApiClient)
        {
            _context = context;
            _clientApiClient = clientApiClient;
        }

        #region Clients
        // Find clients with client name or id
        [HttpGet("filter")]
        public async Task<IActionResult> GetClientsPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.Clients.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.ClientId.Contains(filter) || x.ClientName.Contains(filter));

            }
            var totalReconds = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ClientQuickViewModels()
                {                    
                    ClientId = x.ClientId,
                    ClientName = x.ClientName,
                    LogoUri = x.LogoUri
                }).ToListAsync();

            var pagination = new Pagination<ClientQuickViewModels>
            {
                Items = items,
                TotalRecords = totalReconds
            };
            return Ok(pagination);
        }

        [HttpPost]
        public async Task<IActionResult> PostClient([FromBody] ClientRequestModel request)
        {
            var result = await _clientApiClient.PostClient(request);
            if (result == true)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("{clientId}")]
        public async Task<IActionResult> DeleteClient(string clientId)
        {
            var result = await _clientApiClient.DeleteClient(clientId);
            if (result == true)
            {
                return Ok();
            }
            return BadRequest();
        }
        #endregion

        #region Basic Setting
        //Get basic infor client for edit
        [HttpGet("{clientId}/basics")]
        public async Task<IActionResult> GetClientBasic(string clientId)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
            {
                return NotFound();
            }
            var origins = await _context.ClientCorsOrigins
                   .Where(x => x.ClientId == client.Id)
                   .Select(x => x.Origin.ToString()).ToListAsync();
            var clientBasic = new ClientBasicViewModel()
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                ClientUri = client.ClientUri,
                Description = client.Description,
                LogoUri = client.LogoUri,
                AllowedCorsOrigins = origins
            };
            return Ok(clientBasic);
        }

        [HttpPut("{clientId}/basics")]
        public async Task<IActionResult> PutClientBasic(string clientId, [FromBody] ClientBasicRequestModel request)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
            {
                return NotFound();
            }
            client.Description = request.Description;
            client.ClientUri = request.ClientUri;
            client.LogoUri = request.LogoUri;
            client.Updated = DateTime.UtcNow;
            _context.Clients.Update(client);

            var origins = await _context.ClientCorsOrigins
                  .Where(x => x.ClientId == client.Id)
                  .Select(x => x.Origin.ToString()).ToListAsync();
            foreach (var origin in origins)
            {
                if (!(request.AllowedCorsOrigins.Contains(origin)))
                {
                    var removeOrigin = await _context.ClientCorsOrigins.FirstOrDefaultAsync(x => x.Origin == origin);
                    _context.ClientCorsOrigins.Remove(removeOrigin);
                }
            }

            foreach (var requestOrigin in request.AllowedCorsOrigins)
            {
                if (!origins.Contains(requestOrigin))
                {
                    var addOrigin = new ClientCorsOrigin()
                    {
                        Origin = requestOrigin,
                        ClientId = client.Id
                    };
                    _context.ClientCorsOrigins.Add(addOrigin);
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

        #region Setting

        // Get all scopes
        [HttpGet]
        public async Task<IActionResult> GetScopes()
        {
            var apiScope = await _context.ApiScopes
                .Select(x => x.Name.ToString()).ToListAsync();
            var identity = await _context.IdentityResources
                .Select(x => x.Name.ToString()).ToListAsync();
            var offlineAccess =new List<string>() { "offline_access" };
            var scopes = (identity.Concat(offlineAccess)).Concat(apiScope);
            return Ok(scopes);
        }

        //Get setting infor client for edit
        [HttpGet("{clientId}/settings")]
        public async Task<IActionResult> GetClientSetting(string clientId)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
            {
                return NotFound();
            }
            var allowedScopes = await _context.ClientScopes
                .Where(x => x.ClientId == client.Id)
                .Select(x => x.Scope.ToString()).ToListAsync();
            var redirectUris = await _context.ClientRedirectUris
                .Where(x => x.ClientId == client.Id)
                .Select(x => x.RedirectUri.ToString()).ToListAsync();
            var allowedGrantTypes = await _context.ClientGrantTypes
                .Where(x => x.ClientId == client.Id)
                .Select(x => x.GrantType.ToString()).ToListAsync();
            var clientSetting = new ClientSettingViewModel()
            {
                Enabled = client.Enabled,
                AllowedScopes = allowedScopes,
                RedirectUris = redirectUris,
                AllowedGrantTypes = allowedGrantTypes,
                RequireConsent = client.RequireConsent,
                AllowRememberConsent = client.AllowRememberConsent,
                AllowOfflineAccess = client.AllowOfflineAccess,
                RequireClientSecret = client.RequireClientSecret,
                ProtocolType = client.ProtocolType,
                RequirePkce = client.RequirePkce,
                AllowPlainTextPkce = client.AllowPlainTextPkce,
                AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser
            };
            return Ok(clientSetting);
        }

        [HttpPut("{clientId}/settings")]
        public async Task<IActionResult> PutClientSettings(string clientId, [FromBody] ClientSettingRequestModel request)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
            {
                return NotFound();
            }
            client.Enabled = request.Enabled;
            client.RequireConsent = request.RequireConsent;
            client.AllowRememberConsent = request.AllowRememberConsent;
            client.AllowOfflineAccess = request.AllowOfflineAccess;
            client.RequireClientSecret = request.RequireClientSecret;
            client.ProtocolType = request.ProtocolType;
            client.RequirePkce = request.RequirePkce;
            client.AllowPlainTextPkce = request.AllowPlainTextPkce;
            client.AllowAccessTokensViaBrowser = request.AllowAccessTokensViaBrowser;
            client.Updated = DateTime.UtcNow;
            _context.Clients.Update(client);
            // Scope
            var allowedScopes = await _context.ClientScopes
                  .Where(x => x.ClientId == client.Id)
                  .Select(x => x.Scope.ToString()).ToListAsync();
            foreach (var allowedScope in allowedScopes)
            {
                if (!(request.AllowedScopes.Contains(allowedScope)))
                {
                    var removeAllowedScope = await _context.ClientScopes.FirstOrDefaultAsync(x => x.Scope == allowedScope);
                    _context.ClientScopes.Remove(removeAllowedScope);
                }
            }

            foreach (var requestAllowedScope in request.AllowedScopes)
            {
                if (!allowedScopes.Contains(requestAllowedScope))
                {
                    var addAllowedScope = new ClientScope()
                    {
                        Scope = requestAllowedScope,
                        ClientId = client.Id
                    };
                    _context.ClientScopes.Add(addAllowedScope);
                }
            }
            // RedirectUris
            var redirectUris = await _context.ClientRedirectUris
              .Where(x => x.ClientId == client.Id)
              .Select(x => x.RedirectUri.ToString()).ToListAsync();
            foreach (var redirectUri in redirectUris)
            {
                if (!(request.RedirectUris.Contains(redirectUri)))
                {
                    var removeRedirectUri = await _context.ClientRedirectUris.FirstOrDefaultAsync(x => x.RedirectUri == redirectUri);
                    _context.ClientRedirectUris.Remove(removeRedirectUri);
                }
            }

            foreach (var requestRedirectUri in request.RedirectUris)
            {
                if (!redirectUris.Contains(requestRedirectUri))
                {
                    var addRedirectUri = new ClientRedirectUri()
                    {
                        RedirectUri = requestRedirectUri,
                        ClientId = client.Id
                    };
                    _context.ClientRedirectUris.Add(addRedirectUri);
                }
            }

            //AllowedGrantTypes
            var allowedGrantTypes = await _context.ClientGrantTypes
                 .Where(x => x.ClientId == client.Id)
                 .Select(x => x.GrantType.ToString()).ToListAsync();
            foreach (var allowedGrantType in allowedGrantTypes)
            {
                if (!(request.AllowedGrantTypes.Contains(allowedGrantType)))
                {
                    var removeAllowedGrantTypes = await _context.ClientGrantTypes.FirstOrDefaultAsync(x => x.GrantType == allowedGrantType);
                    _context.ClientGrantTypes.Remove(removeAllowedGrantTypes);
                }
            }

            foreach (var requestAllowedGrantType in request.AllowedGrantTypes)
            {
                if (!allowedGrantTypes.Contains(requestAllowedGrantType))
                {
                    var addAllowedGrantTypes = new ClientGrantType()
                    {
                        GrantType = requestAllowedGrantType,
                        ClientId = client.Id
                    };
                    _context.ClientGrantTypes.Add(addAllowedGrantTypes);
                }
            }
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();

        }

        // Client Secrets
        [HttpGet("{clientId}/settings/clientSecrets")]
        public async Task<IActionResult> GetClientSecrets(string clientId)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
                return NotFound();
            var query = _context.ClientSecrets.Where(x => x.ClientId.Equals(client.Id));
            var clientSecrets = await query.Select(x => new ClientSecretViewModels()
            {
                Id = x.Id,
                Value = x.Value,
                Type = x.Type,
                Expiration = x.Expiration.ToString().Substring(0, 10),
                Description = x.Description
            }).ToListAsync();

            return Ok(clientSecrets);
        }

        // Post client secrets
        [HttpPost("{clientId}/settings/clientSecrets")]
        public async Task<IActionResult> PostClientSecret(string clientId, [FromBody] ClientSecretRequestModel request)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
            {
                return NotFound();
            }
            var clientSecret = new ClientSecret()
            {
                Type = "SharedSecret",
                Value = request.Value.ToSha256(),
                Description = request.Description,
                ClientId = client.Id,
                Expiration = request.Expiration,
                Created = DateTime.UtcNow
            };
            client.Updated = DateTime.UtcNow;
            _context.Clients.Update(client);
            _context.ClientSecrets.Add(clientSecret);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }

        //Delete client secret
        [HttpDelete("{clientId}/settings/clientSecrets/{secretId}")]
        public async Task<IActionResult> DeleteClientSecret(string clientId, int secretId)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
                return NotFound();
            var clientSecret = await _context.ClientSecrets.FirstOrDefaultAsync(x => x.ClientId == client.Id && x.Id == secretId);
            if (clientSecret == null)
                return NotFound();

            _context.ClientSecrets.Remove(clientSecret);
            client.Updated = DateTime.UtcNow;
            _context.Clients.Update(client);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }

        // Client Properties
        [HttpGet("{clientId}/settings/properties")]
        public async Task<IActionResult> GetClientProperties(string clientId)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
                return NotFound();
            var query = _context.ClientProperties.Where(x => x.ClientId.Equals(client.Id));
            var clientProperties = await query.Select(x => new ClientPropertyViewModels()
            {
                Id = x.Id,
                Key = x.Key,
                Value = x.Value
            }).ToListAsync();
            return Ok(clientProperties);
        }

        // Post client Property
        [HttpPost("{clientId}/settings/properties")]
        public async Task<IActionResult> PostClientProperty(string clientId, [FromBody] ClientPropertyRequestModel request)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
            {
                return NotFound();
            }
            var clientProperty = await _context.ClientProperties.FirstOrDefaultAsync(x => x.Key == request.Key && x.ClientId == client.Id);
            if (clientProperty != null)
            {
                return BadRequest();
            }
            var clientPropertyRequest = new ClientProperty()
            {
                Key = request.Key,
                Value = request.Value,
                ClientId = client.Id
            };
            _context.ClientProperties.Add(clientPropertyRequest);
            client.Updated = DateTime.UtcNow;
            _context.Clients.Update(client);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }

        //Delete client property
        [HttpDelete("{clientId}/settings/properties/{propertyKey}")]
        public async Task<IActionResult> DeleteClientProperty(string clientId, string propertyKey)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
                return NotFound();
            var clientProperty = await _context.ClientProperties.FirstOrDefaultAsync(x => x.ClientId == client.Id && x.Key == propertyKey);
            if (clientProperty == null)
                return NotFound();

            _context.ClientProperties.Remove(clientProperty);
            client.Updated = DateTime.UtcNow;
            _context.Clients.Update(client);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }
        #endregion

        #region Authentication Setting
        //Get Authentication infor client for edit
        [HttpGet("{clientId}/authentications")]
        public async Task<IActionResult> GetClientAuthentication(string clientId)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
            {
                return NotFound();
            }
            var postLogoutRedirectUris = await _context.ClientPostLogoutRedirectUris
                    .Where(x => x.ClientId == client.Id)
                    .Select(x => x.PostLogoutRedirectUri.ToString()).ToListAsync();
            var clientAuthentication = new ClientAuthenticationViewModel()
            {
                EnableLocalLogin = client.EnableLocalLogin,
                PostLogoutRedirectUris = postLogoutRedirectUris,
                FrontChannelLogoutUri = client.FrontChannelLogoutUri,
                FrontChannelLogoutSessionRequired = client.FrontChannelLogoutSessionRequired,
                BackChannelLogoutUri = client.BackChannelLogoutUri,
                BackChannelLogoutSessionRequired = client.BackChannelLogoutSessionRequired,
                UserSsoLifetime = client.UserSsoLifetime

            };
            return Ok(clientAuthentication);
        }

        [HttpPut("{clientId}/authentications")]
        public async Task<IActionResult> PutClientAuthentication(string clientId, [FromBody] ClientAuthenticationRequestModel request)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
            {
                return NotFound();
            }
            client.EnableLocalLogin = request.EnableLocalLogin;
            client.FrontChannelLogoutUri = request.FrontChannelLogoutUri;
            client.FrontChannelLogoutSessionRequired = request.FrontChannelLogoutSessionRequired;
            client.BackChannelLogoutUri = request.BackChannelLogoutUri;
            client.BackChannelLogoutSessionRequired = request.BackChannelLogoutSessionRequired;
            client.UserSsoLifetime = request.UserSsoLifetime;
            client.Updated = DateTime.UtcNow;
            _context.Clients.Update(client);

            var postLogoutRedirectUris = await _context.ClientPostLogoutRedirectUris
                  .Where(x => x.ClientId == client.Id)
                  .Select(x => x.PostLogoutRedirectUri.ToString()).ToListAsync();
            foreach (var postLogoutRedirectUri in postLogoutRedirectUris)
            {
                if (!(request.PostLogoutRedirectUris.Contains(postLogoutRedirectUri)))
                {
                    var removePostLogoutRedirectUri = await _context.ClientPostLogoutRedirectUris.FirstOrDefaultAsync(x => x.PostLogoutRedirectUri == postLogoutRedirectUri);
                    _context.ClientPostLogoutRedirectUris.Remove(removePostLogoutRedirectUri);
                }
            }

            foreach (var requestPostLogoutRedirectUri in request.PostLogoutRedirectUris)
            {
                if (!postLogoutRedirectUris.Contains(requestPostLogoutRedirectUri))
                {
                    var addPostLogoutRedirectUri = new ClientPostLogoutRedirectUri()
                    {
                        PostLogoutRedirectUri = requestPostLogoutRedirectUri,
                        ClientId = client.Id
                    };
                    _context.ClientPostLogoutRedirectUris.Add(addPostLogoutRedirectUri);
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

        #region Token Setting
        //Get token infor client for edit
        [HttpGet("{clientId}/tokens")]
        public async Task<IActionResult> GetClientToken(string clientId)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
            {
                return NotFound();
            }
            var clientToken = new ClientTokenViewModel()
            {
                IdentityTokenLifetime = client.IdentityTokenLifetime,
                AccessTokenLifetime = client.AccessTokenLifetime,
                AccessTokenType = (client.AccessTokenType == 0) ? "Jwt" : "Reference",
                AuthorizationCodeLifetime = client.AuthorizationCodeLifetime,
                AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime,
                SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime,
                RefreshTokenUsage = (client.RefreshTokenUsage == 0) ? "ReUse" : "OneTimeOnly",
                RefreshTokenExpiration = (client.RefreshTokenUsage == 0) ? "Sliding" : "Absolute",
                UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh,
                IncludeJwtId = client.IncludeJwtId,
                AlwaysSendClientClaims = client.AlwaysSendClientClaims,
                AlwaysIncludeUserClaimsInIdToken = client.AlwaysIncludeUserClaimsInIdToken,
                PairWiseSubjectSalt = client.PairWiseSubjectSalt,
                ClientClaimsPrefix = client.ClientClaimsPrefix
            };
            return Ok(clientToken);
        }

        [HttpPut("{clientId}/tokens")]
        public async Task<IActionResult> PutClientToken(string clientId, [FromBody] ClientTokenRequestModel request)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
            {
                return NotFound();
            }
            client.IdentityTokenLifetime = request.IdentityTokenLifetime;
            client.AccessTokenLifetime = request.AccessTokenLifetime;
            client.AccessTokenType = request.AccessTokenType.Contains("Jwt") ? 0 : 1;
            client.AuthorizationCodeLifetime = request.AuthorizationCodeLifetime;
            client.AbsoluteRefreshTokenLifetime = request.AbsoluteRefreshTokenLifetime;
            client.SlidingRefreshTokenLifetime = request.SlidingRefreshTokenLifetime;
            client.RefreshTokenUsage = request.RefreshTokenUsage.Contains("ReUse") ? 0 : 1;
            client.RefreshTokenExpiration = request.RefreshTokenExpiration.Contains("Sliding") ? 0 : 1;
            client.UpdateAccessTokenClaimsOnRefresh = request.UpdateAccessTokenClaimsOnRefresh;
            client.IncludeJwtId = request.IncludeJwtId;
            client.AlwaysSendClientClaims = request.AlwaysSendClientClaims;
            client.AlwaysIncludeUserClaimsInIdToken = request.AlwaysIncludeUserClaimsInIdToken;
            client.PairWiseSubjectSalt = request.PairWiseSubjectSalt;
            client.ClientClaimsPrefix = request.ClientClaimsPrefix;
            client.Updated = DateTime.UtcNow;
            _context.Clients.Update(client);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();

        }

        // Client Claim
        [HttpGet("{clientId}/tokens/clientClaims")]
        public async Task<IActionResult> GetClientClientClaims(string clientId)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
                return NotFound();
            var query = _context.ClientClaims.Where(x => x.ClientId.Equals(client.Id));
            var clientClaims = await query.Select(x => new ClientClaimViewModel()
            {
                Id = x.Id,
                Value = x.Value,
                Type = x.Type,
            }).ToListAsync();

            return Ok(clientClaims);
        }

        // Post client claim
        [HttpPost("{clientId}/tokens/clientClaims")]
        public async Task<IActionResult> PostClientClaim(string clientId, [FromBody] ClientClaimRequestModel request)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
            {
                return NotFound();
            }
            var clientClaim = await _context.ClientClaims.FirstOrDefaultAsync(x => x.Type == request.Type && x.ClientId == client.Id);
            if (clientClaim != null)
            {
                return BadRequest();
            }
            var clientClaimRequest = new ClientClaim()
            {
                Type = request.Type,
                Value = request.Value,
                ClientId = client.Id
            };
            _context.ClientClaims.Add(clientClaimRequest);
            client.Updated = DateTime.UtcNow;
            _context.Clients.Update(client);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }

        //Delete client claim
        [HttpDelete("{clientId}/tokens/clientClaims/{claimType}")]
        public async Task<IActionResult> DeleteClientClaim(string clientId, string claimType)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
                return NotFound();
            var clientClaim = await _context.ClientClaims.FirstOrDefaultAsync(x => x.ClientId == client.Id && x.Type == claimType);
            if (clientClaim == null)
                return NotFound();
            _context.ClientClaims.Remove(clientClaim);
            client.Updated = DateTime.UtcNow;
            _context.Clients.Update(client);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }
        #endregion

        #region Device Flow Setting
        //Get Device Flows infor client for edit
        [HttpGet("{clientId}/deviceFlows")]
        public async Task<IActionResult> GetClientDeviceFlow(string clientId)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
            {
                return NotFound();
            }
            var clientDeviceFlow = new ClientDeviceFlowViewModel()
            {
                UserCodeType = client.UserCodeType,
                DeviceCodeLifetime = client.DeviceCodeLifetime
            };
            return Ok(clientDeviceFlow);
        }

        [HttpPut("{clientId}/deviceFlows")]
        public async Task<IActionResult> PutClientDeviceFlow(string clientId, [FromBody] ClientDeviceFlowRequestModel request)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null)
                return NotFound();
            client.UserCodeType = request.UserCodeType;
            client.DeviceCodeLifetime = client.DeviceCodeLifetime;
            client.Updated = DateTime.UtcNow;
            _context.Clients.Update(client);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return Ok();
            return BadRequest();
        }
        #endregion            
    }
}
