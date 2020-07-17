using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin_api.Data.Entities
{
    public class ApiScopeProperty : Property
    {
        public int ScopeId { get; set; }
        public ApiScope Scope { get; set; }
    }
}
