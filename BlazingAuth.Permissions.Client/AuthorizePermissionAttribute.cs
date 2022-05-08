using Microsoft.AspNetCore.Authorization;
using System;

namespace BlazingAuth.Permissions.Client
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizePermissionAttribute : AuthorizeAttribute
    {
        public string[] OrPermissons { get; }

        public AuthorizePermissionAttribute(params string[] orPermissons) : base(BlazingAuthPolicies.Permission)
        {
            OrPermissons = orPermissons;
        }
    }
}
