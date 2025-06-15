using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CodeHub.Controllers
{
    public class AdminController : Controller
    {
        private readonly BattlesContext _battlesContext;
        private readonly CodeHubDbContext _dbContext;
        public AdminController(BattlesContext battlesContext,CodeHubDbContext dbContext)
        {
            _battlesContext = battlesContext;
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Battles()
        {
            return PartialView();
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ActiveUsers = await _dbContext.Users.CountAsync()-1;
            ViewBag.Battles = await _dbContext.Battles.CountAsync();
            ViewBag.Courses = await _dbContext.Courses.CountAsync();
            ViewBag.ForumPosts = 1892; 

            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBattle(
            string title,
            string description,
            DateTime startDate,
            DateTime endDate,
            int rewardXP)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                    return BadRequest("Името на двубоя е задължително!");

                if (startDate >= endDate)
                    return BadRequest("Крайната дата трябва да бъде след началната дата!");

                if (rewardXP <= 0)
                    return BadRequest("Наградата трябва да бъде положително число!");

                var battle = new Battle
                {
                    Title = title,
                    Description = description,
                    StartDate = startDate,
                    EndDate = endDate,
                    RewardXP = rewardXP,
                };

                _battlesContext.Create(battle);

                return Ok(new { success = true, message = "Двубоят е създаден успешно!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Грешка при създаване на двубой: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult GetAllBattles()
        {
            try
            {
                var battles = _battlesContext.ReadAll();
                return Ok(battles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Грешка при извличане на двубои: {ex.Message}");
            }
        }
    }
}