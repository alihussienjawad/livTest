 
using System.ComponentModel.DataAnnotations;
 
namespace ClassLibrary.Models.VM
{
    public class DeleteAccountDto
    {
  
        public string Id { get; set; } = string.Empty;
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
 

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

 


        

    }
}
