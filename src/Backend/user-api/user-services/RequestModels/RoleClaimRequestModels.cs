using System.Collections.Generic;

namespace user_services.RequestModels
{
    public class RoleClaimRequestModels<T>
    {
        public List<T> Claims { get; set; }
    }
}
