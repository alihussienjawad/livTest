 
using Microsoft.AspNetCore.Mvc;
 
 
using Microsoft.AspNetCore.Identity;
 
 
using ClassLibrary.Data;
using Microsoft.EntityFrameworkCore;
 
using ClassLibrary.Models;
using ClassLibrary.Models.VM;

namespace BackOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IUserStore<ApplicationUser> userStore;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext contex;
        private readonly IUserEmailStore<ApplicationUser> emailStore;

        private readonly IWebHostEnvironment env;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration config;

        public CustomerController(IUserStore<ApplicationUser> userStore,
               UserManager<ApplicationUser> userManager,
               RoleManager<IdentityRole> roleManager,
               ApplicationDbContext contex,
               IWebHostEnvironment env,
               SignInManager<ApplicationUser> signInManager,
               IConfiguration config)
        {
            this.userStore = userStore;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.contex = contex;
            emailStore = GetEmailStore();
            this.env = env;
            this.signInManager = signInManager;
            this.config = config;
        }

        [HttpGet("get-user/{email}")]
        public async Task<ActionResult<UserDto>> GetUserAsync(string email)
        {
             UserDto  userDtos = new();
            var user =await userManager.FindByEmailAsync(email);
           
            if (user is not null)
            {
                 
                    userDtos =new()
                    {
                       Id=user.Id,
                       Email=user.Email??string.Empty,
                       FirstName=user.FirstName,
                       LastName=user.LastName,
                       FullName=$"{user.FirstName}  {user.FirstName}",
                       Address1=user.Address1,
                       Address2=user.Address2,
                       Bio=user.Bio,
                       City=user.City,
                       PhoneNumber=user.Phone,
                       PersonImg= user.PersonImg
                    };
                 
            }
            return Ok(userDtos);
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

        [HttpPut("Update-info")]
        public async Task<ActionResult<UserDto>> UpdateinfoAsync(UserDto model)
        {
            ApplicationUser? identity = await contex.ApplicationUsers.FindAsync(model.Id);
            if(identity != null)
            {
                if(model.Imagefile.base64data is not null)
                {
                    string baseUrl = config["BaseAddress"] ?? "";
                    if (!Directory.Exists(env.WebRootPath + "/profiles/personsImg"))
                    {
                        Directory.CreateDirectory(env.WebRootPath + "/profiles/personsImg/");
                    }
                    string path = "\\profiles\\personsImg\\" + model.Imagefile.fileName;
                    var buf = Convert.FromBase64String(model.Imagefile.base64data);

                    if (System.IO.File.Exists(env.WebRootPath+path))
                           System.IO.File.Delete(env.WebRootPath + path);

                    await System.IO.File.WriteAllBytesAsync(env.WebRootPath + path, buf);
                    identity.PersonImg = baseUrl+path;


                }


                identity.FirstName = model.FirstName;
                identity.LastName = model.LastName;
                identity.Bio = model.Bio;
                identity.Phone = model.PhoneNumber!;


                   contex.ApplicationUsers.Update(identity);
                   await contex.SaveChangesAsync();
                   return Ok(model);
               

            }
            return BadRequest();

        }

        [HttpPut("Update-info-Address")]
        public async Task<ActionResult<UserDto>> UpdateinfoAddressAsync(UserDto model)
        {
            ApplicationUser? identity = await contex.ApplicationUsers.FindAsync(model.Id);
            if (identity != null)
            {
                identity.Address1 = model.Address1;
                identity.Address2 = model.Address2;
                identity.City = model.City;
                identity.PostalCode = model.PostalCode;



                contex.ApplicationUsers.Update(identity);
                await contex.SaveChangesAsync();
                return Ok(model);


            }
            return BadRequest();

        }
        [HttpPut("ChangePassword")]
        public async Task<ActionResult<UserDto>> ChangePasswordAsync(ChangePasswordDto model)
        {
            ApplicationUser? identity = await contex.ApplicationUsers.FindAsync(model.Id);
            if (identity != null)
            {
               var res= await signInManager.PasswordSignInAsync(model.Email, model.OldPassword, true, lockoutOnFailure: false);
              
                if(res.Succeeded)
                {
                    if(model.Password==model.ConfirmPassword)
                    {
                        await userManager.RemovePasswordAsync(identity);
                      var re=   await userManager.AddPasswordAsync(identity, model.Password);
                        if(re.Succeeded)
                        {
                            return Ok();
                        }
                        return BadRequest();
                    }
                    return BadRequest();

                }

                return BadRequest();


            }
            return BadRequest();

        }

        [HttpPut("DeleteAccount")]
        public async Task<ActionResult<UserDto>> DeleteAccountAsync(DeleteAccountDto model)
        {
            ApplicationUser? identity = await contex.ApplicationUsers.FindAsync(model.Id);
            if (identity != null)
            {
                if (model.Password is not null)
                {
                    var res = await signInManager.PasswordSignInAsync(model.Email, model.Password, true, lockoutOnFailure: false);

                    if (res.Succeeded)
                    {
                         
                            
                            var re = await userManager.DeleteAsync(identity);
                            if (re.Succeeded)
                            {
                                return Ok();
                            }
                            return BadRequest();
                        }
                        return BadRequest();

                    }

                }

                return BadRequest();


         

        }




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
