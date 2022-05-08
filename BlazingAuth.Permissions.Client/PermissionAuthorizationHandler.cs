using BlazingAuth.Permissions.Requirements;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazingAuth.Permissions.Client
{
    public class PermissionAuthorizationHandler : AttributeAuthorizationHandler<PermissionAuthorizationRequirement, AuthorizePermissionAttribute>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement, IEnumerable<AuthorizePermissionAttribute> attributes)
        {
            foreach (var permissionAttribute in attributes)
            {
                if (!context.User.HasAnyPermission(permissionAttribute.OrPermissons))
                {
                    context.Fail(new AuthorizationFailureReason(this, "User doesn't have the required permissions."));
                    return Task.CompletedTask;
                }
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement, AuthorizePermissionView permissionsView)
        {
            bool andPermissionsValid = permissionsView.AndPermissionsList is null || context.User.HasPermissions(permissionsView.AndPermissionsList);
            bool orPermissionsValid = permissionsView.OrPermissionsList is null || context.User.HasAnyPermission(permissionsView.OrPermissionsList);

            if (!andPermissionsValid || !orPermissionsValid)
            {
                context.Fail(new AuthorizationFailureReason(this, "User doesn't have the required permissions."));
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
