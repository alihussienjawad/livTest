using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Models
{
    public class CourseLearn
    {
          public int Id { get; set; }
          public string Name { get; set; } = null!;

          [ForeignKey("CourseId")]
          public int CourseId { get; set; }
          public virtual Course? Course { get; private set; }
    }
}
