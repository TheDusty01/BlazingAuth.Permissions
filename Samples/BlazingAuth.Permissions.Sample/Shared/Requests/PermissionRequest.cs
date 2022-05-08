using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazingAuth.Permissions.Sample.Shared.Requests
{
    public class PermissionRequest
    {
        [Required]
        public string? Permission { get; set; }
    }
}
