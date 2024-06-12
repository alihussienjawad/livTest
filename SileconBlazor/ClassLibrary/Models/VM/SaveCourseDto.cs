using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models.VM
{
    public  class SaveCourseDto
    {
        public int CourseId { get; set; }
        public int IsSaved { get; set; }

        public string UserId { get; set; }
    }
}
