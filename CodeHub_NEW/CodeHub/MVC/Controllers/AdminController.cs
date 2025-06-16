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
        private readonly CoursesContext _coursesContext;
        public AdminController(BattlesContext battlesContext, 
            CodeHubDbContext dbContext,CoursesContext coursesContext)
        {
            _battlesContext = battlesContext;
            _dbContext = dbContext;
            _coursesContext = coursesContext;
        }
        public async Task<IActionResult> Battles()
        {
            return PartialView();
        }

        public async Task<IActionResult> Courses()
        {
            return PartialView();
        }
        public async Task<IActionResult> Forum()
        {
            return PartialView();
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.ActiveUsers = await _dbContext.Users.CountAsync() - 1;
            ViewBag.Battles = await _dbContext.Battles.CountAsync();
            ViewBag.Courses = await _dbContext.Courses.CountAsync();
            ViewBag.ForumPosts = await _dbContext.Forums.CountAsync();

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
        public async Task<IActionResult> GetAllBattles()
        {
            try
            {
                var battles = await _battlesContext.ReadAll();
                return Ok(battles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Грешка при извличане на двубои: {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            try
            {
                var courses = _coursesContext.ReadAll(useNavigationalProperties: true, isReadOnly: true);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Грешка при извличане на курсове: {ex.Message}");
            }
        }
        [HttpPost]
        public IActionResult CreateCourse([FromBody] Course course)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(course.Name))
                    return BadRequest("Заглавието на курса е задължително!");

                if (string.IsNullOrWhiteSpace(course.Description))
                    return BadRequest("Описанието на курса е задължително!");

                // Validate Lectors (if provided)
                if (course.Lectors != null)
                {
                    foreach (var lector in course.Lectors)
                    {
                        if (lector.Id <= 0)
                            return BadRequest("Всеки лектор трябва да има валидно ID.");
                    }
                }

                // Validate Lessons (if provided)
                if (course.Lessons != null)
                {
                    foreach (var lesson in course.Lessons)
                    {
                        if (lesson.Id <= 0)
                            return BadRequest("Всеки урок трябва да има валидно ID.");
                    }
                }

                _coursesContext.Create(course);

                return Ok(new { success = true, message = "Курсът е създаден успешно!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Грешка при създаване на курс: {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllLessons()
        {
            try
            {
                var lessons = await _dbContext.Lessons.ToListAsync();
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Грешка при извличане на уроци: {ex.Message}");
            }
        }
    }
}