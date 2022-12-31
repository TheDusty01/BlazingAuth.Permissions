using Microsoft.AspNetCore.Authorization;
using BlazingAuth.Permissions.Requirements;

namespace BlazingAuth.Permissions;

public class BlazingAuthPolicies
{
    public const string Permission = nameof(Permission);
    public static AuthorizationPolicy PermissionPolicy => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new PermissionAuthorizationRequirement())
        .Build();
}
