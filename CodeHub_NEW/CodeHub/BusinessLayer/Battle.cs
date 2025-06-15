using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Battle
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public User FirstPlayer { get; set; }
        public User SecondPlayer { get; set; }
        public int RewardXP { get; set; }

        public Battle()
        {
        }
        public Battle(string title, string description, DateTime startDate, DateTime endDate, User firstPlayer, User secondPlayer, int rewardXP)
        {
            Title = title;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;
            RewardXP = rewardXP;
        }
    }
}
