using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazingAuth.Permissions.Sample.Shared
{
    public class AppPermissions : BlazingAuthClaims.Permission
    {
        public const string ViewAllUsers = "ViewAllUsers";
        public const string ViewAllPermissions = "ViewAllPermissions";
        public const string ManagePermissions = "ManagePermissions";
    }
}
