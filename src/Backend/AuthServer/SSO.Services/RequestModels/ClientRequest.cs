using System;
using System.Collections.Generic;
using System.Text;

namespace SSO.Services.RequestModels
{
    public class ClientRequest
    {
        public string ClientName { get; set; }
        public string Description { get; set; }
        public string ClientUri { get; set; }
        public string LogoUri { get; set; }
        public string ClientType { get; set; }
    }
}
