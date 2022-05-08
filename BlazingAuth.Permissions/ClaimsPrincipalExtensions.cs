using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace BlazingAuth.Permissions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public static IEnumerable<Claim> GetPermissions(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == BlazingAuthClaims.Permission.Type);
        }

        public static IEnumerable<string> GetPermissionValues(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == BlazingAuthClaims.Permission.Type)
                .Select(x => x.Value);
        }

        public static bool HasPermissions(this ClaimsPrincipal claimsPrincipal, params string[] andPermissions)
        {
            var userPermissions = claimsPrincipal.GetPermissionValues();

            foreach (var neededPermission in andPermissions)
            {
                if (!userPermissions.Contains(neededPermission))
                    return false;
            }

            return true;
        }

        public static bool HasAnyPermission(this ClaimsPrincipal claimsPrincipal, params string[] orPermissions)
        {
            var permissionClaims = claimsPrincipal.GetPermissions();

            foreach (var permissionClaim in permissionClaims)
            {
                if (orPermissions.Contains(permissionClaim.Value))
                    return true;
            }

            return false;
        }
    }
}
