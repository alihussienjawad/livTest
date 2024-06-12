using ClassLibrary.Models.VM;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Models
{
  // Specify the table name for the Course entity
    public class Teacher
    {
        public int Id { get; set; }

        public string TeacherName { get; set; } = null!;

        public string? ImagePath { get; set; }

        [NotMapped]
        public ImageFile? ImageFile { get; set; }

        public ICollection<TeacherCourse>? TeacherCourses { get; set; }

        [NotMapped ]
        public bool IsSelected { get; set; }
    }

    public class Teacher2
    {
        public int Id { get; set; }

        public string TeacherName { get; set; } = null!;

        public string? ImagePath { get; set; }


    }
}
