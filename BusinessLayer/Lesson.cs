using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Lesson
    {
        [Key] public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[] Video { get; set; }
        public List<Exercise> Exercises { get; set; }

        public Lesson()
        {
        }

        public Lesson(string title, string description, byte[] video)
        {
            Title = title;
            Description = description;
            Video = video;
            Exercises = new List<Exercise>();
        }
    }
}
