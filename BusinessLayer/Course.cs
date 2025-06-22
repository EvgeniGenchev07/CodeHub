using System.ComponentModel.DataAnnotations;

namespace BusinessLayer
{
    public class Course
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Filters> Filters { get; set; }
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
            Filters = new List<Filters>();
            Lectors = new List<Lector>();
            Lessons = new List<Lesson>();
            Difficulty = difficulty;
        }
    }
}
