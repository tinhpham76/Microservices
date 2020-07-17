using auth_services.Constants;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
