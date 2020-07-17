using System;
using System.Collections.Generic;
using System.Text;

namespace admin_services.ViewModels
{
    public class ApiResourceSecretViewModels
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Expiration { get; set; }
        public string Description { get; set; }
    }
}
