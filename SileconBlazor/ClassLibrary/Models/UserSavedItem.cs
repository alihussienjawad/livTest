using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Models
{
 
    public class UserSavedItem
    {
        public string ApplicationUserId { get; set; } = null!;
        public ApplicationUser? ApplicationUser { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; }


        public bool Is_Saved { get; set; }
        public bool Is_Liked { get; set;}
    }
}
