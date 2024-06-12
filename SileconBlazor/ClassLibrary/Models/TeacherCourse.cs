using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Models
{
    public class TeacherCourse
    {
        public int Id { get; set; }
        [ForeignKey("CourseId")]
        public int CourseId { get; set; }
        public virtual Course? Course { get; private set; }

        [ForeignKey("TeacherId")]
        public int TeacherId { get; set; }
        public virtual Teacher? Teachers { get; private set; }
    }
}
