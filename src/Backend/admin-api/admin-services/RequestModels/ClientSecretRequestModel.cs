using System;

namespace admin_services.RequestModels
{
    public class ClientSecretRequestModel
    {
        public string Value { get; set; }

        public string Type { get; set; }

        public string HashType { get; set; }

        public DateTime? Expiration { get; set; }
        public string Description { get; set; }
    }
}
