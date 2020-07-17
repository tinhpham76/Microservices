﻿using System.Collections.Generic;

namespace admin_services
{
    public class Pagination<T>
    {
        public List<T> Items { get; set; }

        public int TotalRecords { get; set; }
    }
}
