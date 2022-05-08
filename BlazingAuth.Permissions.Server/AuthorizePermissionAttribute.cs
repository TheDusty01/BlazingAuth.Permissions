using Microsoft.AspNetCore.Mvc;

namespace BlazingAuth.Permissions.Server
{
    public class AuthorizePermissionAttribute : TypeFilterAttribute
    {
        public AuthorizePermissionAttribute(params string[] orPermissons) : base(typeof(PermissionAuthorizationFilter))
        {
            Arguments = new object[] { orPermissons };
        }
    }
}
