using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models.VM
{
    public class ImageFile
    {
        public string base64data { get; set; }=string.Empty;
        public string contentType { get; set; }=string.Empty;
        public string fileName { get; set; } = string.Empty;
    }
}
