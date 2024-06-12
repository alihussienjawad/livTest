 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
 
using ClassLibrary.Data;
using ClassLibrary.Models.VM;
using ClassLibrary.Models;

namespace BackOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserStore<ApplicationUser> userStore;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext contex;
        private readonly IUserEmailStore<ApplicationUser> emailStore;
        public UserController(IUserStore<ApplicationUser> userStore,
               UserManager<ApplicationUser> userManager,
               RoleManager<IdentityRole> roleManager,
               ApplicationDbContext contex)
        {
            this.userStore = userStore;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.contex = contex;
            emailStore = GetEmailStore();
        }

        [HttpGet("All-Users")]
        public async Task<ActionResult<List<UserDto>>> GetAllApplicationUserAsync()
        {
            List<UserDto> userDtos = [];
            var users =await contex.ApplicationUsers.ToListAsync();
            if (users.Count != 0)
            {
                foreach (var user in users)
                {
                    userDtos.Add(new()
                    {
                       Id=user.Id,
                       Email=user.Email??string.Empty,
                       FullName=$"{user.FirstName}  {user.FirstName}",
                       Address1=user.Address1,
                       Address2=user.Address2,
                       Bio=user.Bio,
                       City=user.City,
                       PersonImg=user.PersonImg
                    });
                }
            }
            return Ok(users);
        }

        [HttpGet("Single-User/{id}")]
        public async Task<ActionResult<List<UserDto>>> GetSingleApplicationUserAsync(string id)
        {
            UserDto userDtos = new();
            var user =await contex.ApplicationUsers.Where(i=>i.Id==id).FirstOrDefaultAsync();
          
            if (user is not null )
            {
                
                    userDtos=new()
                    {
                        Id = user.Id,
                        Email = user.Email ?? string.Empty,
                        FullName = $"{user.FirstName}  {user.FirstName}",
                        Address1 = user.Address1,
                        Address2 = user.Address2,
                        Bio = user.Bio,
                        City = user.City,
                        PersonImg = user.PersonImg
                    };
               
            }
            return Ok(user);
        }

        [HttpPost("Add-User")]
        public async Task<ActionResult> AddApplicationUserAsync(RegisterUserDto model)
        {
            
            ApplicationUser identity = new()
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
 
            };
            if(ModelState.IsValid)
            {
                await userStore.SetUserNameAsync(identity, model.Email, CancellationToken.None);
                await emailStore.SetEmailAsync(identity, model.Email, CancellationToken.None);

                var result = await userManager.CreateAsync(identity, model.Password);
                if (result.Succeeded)
                {
                    var claims = new List<Claim>
                    {
                         new("firstName",model.FirstName),
                         new("lastName",model.LastName)

                     };
                    await userManager.AddClaimsAsync(identity, claims);

                    await userManager.UpdateAsync(identity);
                    return Ok();
                }


            }
            return BadRequest();

        }

        //[HttpPut("Update-User")]
        //public async Task<ActionResult<List<UserDto>>> UpdateApplicationUserAsync(UserDto model)
        //{
        //    var ApplicationUser = await _userRepo.UpdateUserAsync(model);
        //    return Ok(ApplicationUser);
        //}

        [HttpDelete("Delete-User/{id}")]
        public async Task<ActionResult<List<UserDto>>> DeleteApplicationUserAsync(string id)
        {
            var user =await contex.ApplicationUsers.FindAsync(id);
            if (user == null)
                return BadRequest("User Not Found");
           var res= await  userManager.DeleteAsync(user);
           if(res.Succeeded)
                return Ok();

            return BadRequest("User Not Deleted");
        }
        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)userStore;
        }


        [HttpGet("GetUserRoles/{UserId}")]
        public async Task<ActionResult<UserRoles>> GetUserRoles(string UserId)
        {
            UserRoles userRoles =new();
            ApplicationUser? applicationUser=await contex.ApplicationUsers.FindAsync(UserId);
            if (applicationUser == null) return BadRequest("User Not Found");

            userRoles.UserId = applicationUser.Id;
            userRoles.UserName = applicationUser.Email ?? applicationUser.FullName;

            var roles= await roleManager.Roles.ToListAsync();
            var userRolesOld = await userManager.GetRolesAsync(applicationUser);
            foreach (var role in roles)
            {
                userRoles.Roles.Add(new()
                {
                    RoleId = role.Id,
                    RoleName = role.Name ?? string.Empty,
                    IsSelected = userRolesOld.Any(i=>i==role.Name)
                });
            }
           

            return Ok(userRoles);
        }

   

        [HttpPut("Update-User-Roles")]
        public async Task<ActionResult<UserRoles>> UpdateUserRoles(UserRoles userRoles)
        {
            if (userRoles == null || userRoles.UserId is null) return BadRequest();

            ApplicationUser? applicationUser = await contex.ApplicationUsers.FindAsync(userRoles.UserId);
            if (applicationUser == null) return BadRequest("User Not Found");

            foreach (var role in userRoles.Roles)
            {
                await userManager.RemoveFromRoleAsync(applicationUser,role: role.RoleName);
            }
            foreach (var role in userRoles.Roles.Where(i => i.IsSelected))
            {
                await userManager.AddToRoleAsync(applicationUser, role: role.RoleName);
            }
            return Ok(userRoles);
        }
    }
}
