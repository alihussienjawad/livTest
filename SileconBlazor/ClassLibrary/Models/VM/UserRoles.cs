using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models.VM
{
    public class UserRoles
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public List<RoleDto> Roles { get; set; } = [];
    }
}
