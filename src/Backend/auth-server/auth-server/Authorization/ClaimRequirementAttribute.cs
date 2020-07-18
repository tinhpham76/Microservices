using auth_services.Constants;
using Microsoft.AspNetCore.Mvc;

namespace auth_server.Authorization
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
