
using System.Collections.Generic;

namespace ClassLibrary.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

       public List<Contact>? Contacts { get; set; }

    }
}
