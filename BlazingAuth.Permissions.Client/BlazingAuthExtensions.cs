using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazingAuth.Permissions.Client
{
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
}
