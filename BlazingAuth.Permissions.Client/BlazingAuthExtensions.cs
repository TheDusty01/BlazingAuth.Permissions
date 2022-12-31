using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BlazingAuth.Permissions.Client;

public static class BlazingAuthExtensions
{
    public static IServiceCollection AddBlazingAuthPermissions(this IServiceCollection services)
    {
        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy(BlazingAuthPolicies.Permission, BlazingAuthPolicies.PermissionPolicy);
        });

        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }
}
