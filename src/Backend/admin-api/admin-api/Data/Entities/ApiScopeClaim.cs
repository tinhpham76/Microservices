using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin_api.Data.Entities
{
    public class ApiScopeClaim : UserClaim
    {
        public int ScopeId { get; set; }
        public ApiScope Scope { get; set; }
    }
}
