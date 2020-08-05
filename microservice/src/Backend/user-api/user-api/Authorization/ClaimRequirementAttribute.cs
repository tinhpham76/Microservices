using Microsoft.AspNetCore.Mvc;
using user_services.Constants;

namespace user_api.Authorization
{
    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(PermissionCode permissionId)
            : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { permissionId };
        }
    }
}
