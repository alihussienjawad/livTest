
 
using System.ComponentModel.DataAnnotations;
 

namespace ClassLibrary.Models.VM
{
    public class UserDto
    {
        public string Id { get; set; }=string.Empty;
       
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }=string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName
        {
            get { return FirstName + " " + LastName; }
            set { _ = FirstName + " " + LastName; }
        }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }=string.Empty;


        public string? PersonImg { get; set; }
        public string? Bio { get; set; }
        public string? Address1 { get; set; } 
        public string? Address2 { get; set; }  
        public string? PostalCode { get; set; }  
        public string? City { get; set; }  

        public ImageFile Imagefile { get;set; }

        public string? OldPassword { get; set; }    
    }
}
