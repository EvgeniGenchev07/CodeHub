using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Lector
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Lector()
        {
            
        }
        public Lector(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
