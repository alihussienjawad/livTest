using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
 
 
using ClassLibrary.Models.VM;
 


namespace ClassLibrary.Models
{
  
    public class CourseDto
    {
  
        public int CourseId { get; set; }
        public int CategoryId { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
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

        
        public decimal NumberOfLikesResult { get; set; }


        public ImageFile? ImageFile { get; set; } = new();

        public string CoursDec { get; set; } = string.Empty;
 
        public int Aritc { get; set; }
        public int iDowinload { get; set; }
        public bool Time { get; set; } = true;
        public bool Certifacate { get; set; } = false;
        public List<CourseDetails> CourseDetails { get; set; } = [];
        public   List<CourseLearn> CourseLearns { get; set; } = [];
        public   List<TeacherCourse> TeacherCourses { get; set; }= [];


        public List<Teacher> Teachers { get; set; }= [];
        public List<Category> categories { get; set; } = [];


        public string?CourseImg { get; set; }
    }
}
