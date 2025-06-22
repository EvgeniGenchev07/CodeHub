using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Exercise
    {
        [Key] public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public int Views { get; set; }
        public int Solutions { get; set; }
        public DateTime Date { get; set; }
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
