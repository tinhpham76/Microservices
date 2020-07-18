using System.Collections.Generic;

namespace auth_services.RequestModel
{
    public class RoleClaimRequestModels<T>
    {
        public List<T> Claims { get; set; }
    }
}
