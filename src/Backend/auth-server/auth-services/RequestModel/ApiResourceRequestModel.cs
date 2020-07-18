using System.Collections.Generic;

namespace auth_services.RequestModel
{
    public class ApiResourceRequestModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
        public string AllowedAccessTokenSigningAlgorithms { get; set; }
        public List<string> UserClaim { get; set; }
        public List<string> Scope { get; set; }
    }
}
