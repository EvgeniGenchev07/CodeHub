using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Difficulty Difficulty { get; set; }
        public List<Lector> Lectors { get; set; }
        public List<Lesson> Lessons { get; set; }

        public Course()
        {
            
        }
        public Course(string name, string description, Difficulty difficulty)
        {
            Name = name;
            Description = description;
            Lectors = new List<Lector>();
            Lessons = new List<Lesson>();
            Difficulty = difficulty;
        }

    }
}
