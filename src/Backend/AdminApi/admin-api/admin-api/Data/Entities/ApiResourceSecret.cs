﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin_api.Data.Entities
{
    public class ApiResourceSecret : Secret
    {
        public int ApiResourceId { get; set; }
        public ApiResource ApiResource { get; set; }
    }
}
