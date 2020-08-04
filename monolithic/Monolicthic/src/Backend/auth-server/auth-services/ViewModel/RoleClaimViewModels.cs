using System.Collections.Generic;

namespace auth_services.ViewModel
{
    public class RoleClaimViewModels<T>
    {
        public List<T> Claims { get; set; }
    }
}
