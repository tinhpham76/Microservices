﻿using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SSO.BackendIdentityServer
{
    //Seed data identity server if db is null
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Phone(),
                new IdentityResources.Address()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("AUTH-SERVER", "Auth Server API Resources")
                {
                    ApiSecrets = { new Secret("secret".Sha256()) },
                    Scopes = { "AUTH-SERVER" }
                }                
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                // backward compat
                new ApiScope("AUTH-SERVER")               
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client> {
                new Client
                {
                    ClientId = "swagger_admin",
                    ClientName = "Swagger Admin",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,

                    RedirectUris =           { "https://localhost:5000/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { "https://localhost:5000/swagger" },
                    AllowedCorsOrigins =     { "https://localhost:5000" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "AUTH-SERVER"
                    }
                }
            };
        }
    }
}