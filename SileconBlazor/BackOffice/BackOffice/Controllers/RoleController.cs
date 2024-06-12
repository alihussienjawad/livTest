
using ClassLibrary.Models.VM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
 

namespace BackOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> roleManager;
        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;

        }

        [HttpGet("All-Roles")]
        public async Task<ActionResult<List<UserDto>>> GetAllRolesAsync()
        {
            List<RoleDto> roleDtos = [];
           var roles=await  roleManager.Roles.ToListAsync();
            foreach (var role in roles)
            {
                roleDtos.Add(new()
                {
                    RoleId = role.Id,
                    RoleName = role.Name??string.Empty
                });

            }
            return Ok(roleDtos);
        }


        [HttpPost("Add-Role")]
        public async Task<ActionResult>AddRoleAsync(RoleDto model)
        {
            if(ModelState.IsValid)
            {
                var role=roleManager.Roles.Any(i=>i.Name!.ToLower()==model.RoleName.ToLower()) ;
                if(role)
                    return BadRequest("Role Found");
                IdentityRole identityRole = new()
                {
                    Name = model.RoleName,
                };
                var res= await roleManager.CreateAsync(identityRole);
                if(res.Succeeded)
                {
                    return Ok();
                }
                return BadRequest("Not Added");
            }
            return BadRequest("Not Valid");
        }


        [HttpGet("Singal-Role/{id}")]
        public async Task<ActionResult<RoleDto>> GetRoleAsync(string id)
        {
            var role=await roleManager.FindByIdAsync(id);
            if (role == null) return BadRequest("Not Found");
            RoleDto roleDto = new()
            {
                RoleId = role.Id,
                RoleName = role.Name??string.Empty
            };
            return Ok(roleDto);
        }


        [HttpPut("Update-Role")]
        public async Task<ActionResult<RoleDto>> UpdateRoleAsync(RoleDto role)
        {
            if (ModelState.IsValid)
            {
                IdentityRole? identityRole = await roleManager.FindByIdAsync(role.RoleId);
                if (identityRole == null)
                    return BadRequest();
                identityRole.Name = role.RoleName;
                identityRole.NormalizedName=role.RoleName.ToUpper();
               
               
             var res=   await roleManager.UpdateAsync(identityRole);
                if(res.Succeeded)
                {
                    
                    return Ok(role);
                }
                return BadRequest("Not Updated");
            }
            return BadRequest("Error");
        }

        [HttpDelete("Delete-Role/{id}")]
        public async Task<ActionResult> DeleteRoleAsync(string id)
        {
            if (ModelState.IsValid)
            {
                IdentityRole? identityRole = await roleManager.FindByIdAsync(id);
                if (identityRole == null)
                    return BadRequest();

                var res = await roleManager.DeleteAsync(identityRole);
                if (res.Succeeded)
                {
                    return Ok();
                }
                return BadRequest("Not Deleted");
            }
            return BadRequest("Error");
        }
    }


}
