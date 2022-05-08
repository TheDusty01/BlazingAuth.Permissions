using BlazingAuth.Permissions.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace BlazingAuth.Permissions
{
    public class BlazingAuthPolicies
    {
        public const string Permission = nameof(Permission);
        public static AuthorizationPolicy PermissionPolicy => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionAuthorizationRequirement())
            .Build();
    }
}
