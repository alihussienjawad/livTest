using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
 
 
using ClassLibrary.Models.VM;
 


namespace ClassLibrary.Models
{
    [Table("Courses")]
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [NotMapped]
        public List<Category> categories { get; set; } = [];

        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser? ApplicationUser { get; set; }


        public string CourseName { get; set; } = null!;

        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal PriceAfterDiscount { get; set; }
        public int NumberOfHours { get; set; }
        public int Rate { get; set; }
        public int NumberOfLikes { get; set; }

        [Display(Name = "downloadable resources")]
        public int DowinloadResource { get; set; }

        [NotMapped]
        public decimal NumberOfLikesResult { get; set; }


        public string? ImagePath { get; set; }= string.Empty;
        [NotMapped]
        public ImageFile ImageFile { get; set; } = new();

        public string CoursDec { get; set; } = string.Empty;
 
        public int Aritc { get; set; }
        public int iDowinload { get; set; }

        [Display(Name = "Full Time")]
        public bool Time { get; set; } = true;
        public bool Certifacate { get; set; } = true;

        public ICollection<UserSavedItem> UserSavedItems { get; set; } = [];




        public virtual List<CourseDetails> CourseDetails { get; set; } = [];
        public virtual List<CourseLearn> CourseLearns { get; set; } = [];
        public virtual List<TeacherCourse> TeacherCourses { get; set; } = [];


       
        [NotMapped]
        public List<Teacher2>? Teachers { get; set; } = [];


 

        [NotMapped]
        public bool IsSaved { get; set; }

        [NotMapped]
        public bool IsLiked { get; set; }

        [NotMapped]
        public bool IsBestSeller { get; set; }
    }
}
