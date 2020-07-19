using System;
using System.Collections.Generic;
using System.Text;

namespace user_services.RequestModels
{
    public class RoleClaimRequestModels<T>
    {
        public List<T> Claims { get; set; }
    }
}
