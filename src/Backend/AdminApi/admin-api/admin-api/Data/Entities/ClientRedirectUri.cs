﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin_api.Data.Entities
{
    public class ClientRedirectUri
    {
        public int Id { get; set; }
        public string RedirectUri { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}