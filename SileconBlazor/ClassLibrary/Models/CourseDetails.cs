 
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Models
{
    public class CourseDetails
    {
          public int Id { get; set; }
          public string Name { get; set; } = null!;
          public string Description { get; set; } = null!;

          [ForeignKey("CourseId")]
          public int CourseId { get; set; }
          public Course? Course { get; private set; }
    }
}
