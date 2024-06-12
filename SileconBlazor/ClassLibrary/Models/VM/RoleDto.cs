using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models.VM
{
    public  class RoleDto
    {
        public string RoleId { get; set; }=string.Empty;

        [Required]
        public string RoleName { get; set; } = string.Empty;

        public bool IsSelected { get; set; }
    }
}
