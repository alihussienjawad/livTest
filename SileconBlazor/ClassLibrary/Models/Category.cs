using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Models
{
    [Table("Categories")]  // You can use a different table name if needed
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name ="Cata")]
        public string CategoryName { get; set; } = null!;

        public List<Course>? Courses { get; set; }
    }


}
