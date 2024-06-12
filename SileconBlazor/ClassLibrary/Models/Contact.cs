using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models
{
    public class Contact
    {
      

        public int Id { get; set; }


        [Display(Name ="Full Name")]
        public string FullName { get; set; } = null!;


        [Display(Name = "Services (optional)")]
        public int ServiceId { get; set; }
        public Service? Service { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Display(Name = "Message")]
        public string Message { get; set; } = null!;
    }
}