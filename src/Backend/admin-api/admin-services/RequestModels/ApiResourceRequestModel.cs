using System;
using System.Collections.Generic;
using System.Text;

namespace admin_services.RequestModels
{
    public class ApiResourceRequestModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
        public string AllowedAccessTokenSigningAlgorithms { get; set; }
    }
}
