using System;
using System.Collections.Generic;
using System.Text;

namespace admin_services.RequestModels
{
    public class ClientBasicRequestModel
    {
        public string Description { get; set; }
        public string ClientUri { get; set; }
        public string LogoUri { get; set; }
        public List<string> AllowedCorsOrigins { get; set; }
    }
}
