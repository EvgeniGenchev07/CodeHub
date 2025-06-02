using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public Difficulty Difficulty { get; set; }

        public Exercise()
        {
            
        }
        public Exercise(string title, string description, int points, Difficulty difficulty)
        {
            Title = title;
            Description = description;
            Points = points;
            Difficulty = difficulty;
        }
    }
}
