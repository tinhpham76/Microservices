using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace auth_server.IdentityServer
{
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
                },
                new ApiResource("ADMIN-API", "Admin API Resources")
                {
                    ApiSecrets = { new Secret("secret".Sha256()) },
                    Scopes = { "ADMIN-API" }
                },
                 new ApiResource("USER-API", "User API Resources")
                {
                    ApiSecrets = { new Secret("secret".Sha256()) },
                    Scopes = { "USER-API" }
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                // backward compat
                new ApiScope("AUTH-SERVER"),
                new ApiScope("ADMIN-API"),
                 new ApiScope("USER-API")
            };

        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client> {
                new Client
                {
                    ClientId = "swagger-auth-server",
                    ClientName = "Swagger Auth Server",

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
                },
                new Client
                {
                    ClientId = "swagger-admin-api",
                    ClientName = "Swagger Admin Api",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,

                    RedirectUris =           { "https://localhost:5002/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { "https://localhost:5002/swagger" },
                    AllowedCorsOrigins =     { "https://localhost:5002" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "ADMIN-API",
                        "AUTH-SERVER"
                    }
                },
                new Client
                {
                    ClientId = "swagger-user-api",
                    ClientName = "Swagger User Api",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,

                    RedirectUris =           { "https://localhost:5004/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { "https://localhost:5004/swagger" },
                    AllowedCorsOrigins =     { "https://localhost:5004" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "USER-API",
                        "AUTH-SERVER"
                    }
                },
                new Client
                {
                    ClientName = "Angular Admin Dashboard",
                    ClientId = "angular-admin-dashboard",
                    AccessTokenType = AccessTokenType.Reference,
                    RequireConsent = false,

                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,

                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        "http://localhost:4200/auth-callback",
                        "http://localhost:4200/silent-renew.html"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:4200/"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://localhost:4200"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "AUTH-SERVER",
                        "ADMIN-API",
                        "USER-API"
                    }
                },
                new Client
                {
                    ClientName = "Angular User Profile",
                    ClientId = "angular-user-profile",
                    AccessTokenType = AccessTokenType.Reference,
                    RequireConsent = false,

                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,

                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        "http://localhost:4300/auth-callback",
                        "http://localhost:4300/silent-renew.html"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:4300/"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://localhost:4200"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "AUTH-SERVER",                       
                        "USER-API"
                    }
                }
            };
        }
    }
}
