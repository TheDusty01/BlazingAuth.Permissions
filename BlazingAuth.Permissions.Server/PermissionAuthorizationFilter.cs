using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace BlazingAuth.Permissions.Server
{
    public class PermissionAuthorizationFilter : IAuthorizationFilter
    {
        private readonly string[] orPermissons;

        public PermissionAuthorizationFilter(string[] orPermissons)
        {
            this.orPermissons = orPermissons;
        }

        public virtual void OnAuthorization(AuthorizationFilterContext context)
        {
            var permissionClaims = context.HttpContext.User.Claims.Where(x => x.Type == BlazingAuthClaims.Permission.Type);

            foreach (var permissionClaim in permissionClaims)
            {
                // Exit early if any of the supplied permissions are matched
                if (orPermissons.Contains(permissionClaim.Value))
                {
                    return;
                }
            }

            context.Result = new ForbidResult();
        }
    }
}
