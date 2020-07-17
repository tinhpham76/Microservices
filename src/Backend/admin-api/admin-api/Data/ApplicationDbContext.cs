using admin_api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin_api.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        // Dbset Client
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientCorsOrigin> ClientCorsOrigins { get; set; }
        public DbSet<ClientScope> ClientScopes { get; set; }
        public DbSet<ClientRedirectUri> ClientRedirectUris { get; set; }
        public DbSet<ClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris { get; set; }
        public DbSet<ClientGrantType> ClientGrantTypes { get; set; }
        public DbSet<ClientSecret> ClientSecrets { get; set; }
        public DbSet<ClientClaim> ClientClaims { get; set; }
        public DbSet<ClientProperty> ClientProperties { get; set; }

        public DbSet<IdentityResource> IdentityResources { get; set; }

        // Dbset Api resources
        public DbSet<ApiResource> ApiResources { get; set; }
        public DbSet<ApiResourceClaim> ApiResourceClaims { get; set; }
        public DbSet<ApiResourceSecret> ApiResourceSecrets { get; set; }
        public DbSet<ApiResourceProperty> ApiResourceProperties { get; set; }
        public DbSet<ApiResourceScope> ApiResourceScopes { get; set; }
        public DbSet<ApiScope> ApiScopes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }


    }
}
