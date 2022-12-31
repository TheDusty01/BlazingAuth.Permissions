using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace BlazingAuth.Permissions.Client;

public abstract class AttributeAuthorizationHandler<TRequirement, TAttribute> : AuthorizationHandler<TRequirement>
    where TRequirement : IAuthorizationRequirement
    where TAttribute : Attribute
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
    {
        var attributes = new List<TAttribute>();

        if (context.Resource is null)
            throw new ArgumentNullException(nameof(context.Resource), "Resource is not specified. Did you forget to set Resource=\"@routeData\" ?");

        if (context.Resource is AuthorizePermissionView permissionsView)
        {
            // Custom AuthorizePermissionView component
            return HandleRequirementAsync(context, requirement, permissionsView);
        }
        else if (context.Resource is RouteData routeData && routeData?.PageType is not null)
        {
            // Add all AuthorizePermssion attributes to the list
            attributes.AddRange(GetAttributes(routeData.PageType));
        }

        // Handle all AuthorizePermssion attributes (if any)
        return HandleRequirementAsync(context, requirement, attributes);
    }

    protected abstract Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement, IEnumerable<TAttribute> attributes);
    protected abstract Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement, AuthorizePermissionView permissionsView);

    private static IEnumerable<TAttribute> GetAttributes(MemberInfo memberInfo)
    {
        return memberInfo.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>();
    }
}
