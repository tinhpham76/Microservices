using System;
using System.Collections.Generic;
using System.Text;

namespace admin_services.ViewModels
{
    public class ClientSecretViewModels
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public DateTime? Expiration { get; set; }
        public string Type { get; set; }
    }
}
