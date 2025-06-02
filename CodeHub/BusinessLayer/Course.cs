using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Difficulty Difficulty { get; set; }
        public List<Lector> Lectors { get; set; }
        public List<Lesson> Lessons { get; set; }

        public Course()
        {
            
        }
        public Course(string name, string description, Difficulty difficulty, List<Lector> lectors, List<Lesson> lessons)
        {
            Name = name;
            Description = description;
            Lectors = lectors;
            Lessons = lessons;
            Difficulty = difficulty;
        }

    }
}
