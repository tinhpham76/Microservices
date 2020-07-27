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
                new ApiResource("AUTH_SERVER", "Auth Server API Resources")
                {
                    ApiSecrets = { new Secret("secret".Sha256()) },
                    Scopes = { "AUTH_SERVER" }
                },
                new ApiResource("ADMIN_API", "Admin API Resources")
                {
                    ApiSecrets = { new Secret("secret".Sha256()) },
                    Scopes = { "ADMIN_API" }
                },
                 new ApiResource("USER_API", "User API Resources")
                {
                    ApiSecrets = { new Secret("secret".Sha256()) },
                    Scopes = { "USER_API" }
                }
            };
        }
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                // backward compat
                new ApiScope("AUTH_SERVER"),
                new ApiScope("ADMIN_API"),
                 new ApiScope("USER_API")
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client> {
                new Client
                {
                    ClientId = "swagger_auth_server",
                    ClientName = "Swagger Auth Server",
                    LogoUri = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/ee/.NET_Core_Logo.svg/512px-.NET_Core_Logo.svg.png",

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
                        "AUTH_SERVER"
                    }
                },
                new Client
                {
                    ClientId = "swagger_admin_api",
                    ClientName = "Swagger Admin Api",
                    LogoUri = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/ee/.NET_Core_Logo.svg/512px-.NET_Core_Logo.svg.png",

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
                        "ADMIN_API",
                        "AUTH_SERVER"
                    }
                },
                new Client
                {
                    ClientId = "swagger_user_api",
                    ClientName = "Swagger User Api",
                     LogoUri = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/ee/.NET_Core_Logo.svg/512px-.NET_Core_Logo.svg.png",

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
                        "USER_API",
                        "AUTH_SERVER"
                    }
                },
                new Client
                {
                    ClientName = "Angular Admin Dashboard",
                    ClientId = "angular_admin_dashboard",
                    LogoUri = "https://angular.io/assets/images/logos/angular/angular.svg",
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
                        "AUTH_SERVER",
                        "ADMIN_API",
                        "USER_API"
                    }
                },
                new Client
                {
                    ClientName = "Angular User Profile",
                    ClientId = "angular_user_profile",
                    LogoUri = "https://angular.io/assets/images/logos/angular/angular.svg",
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
                        "http://localhost:4300"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "AUTH_SERVER",
                        "USER_API"
                    }
                }
            };
        }
    }
}
