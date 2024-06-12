using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models.VM
{
    public class DeleteCourseDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }=string.Empty;
        public decimal Price { get; set; }
    }
}
