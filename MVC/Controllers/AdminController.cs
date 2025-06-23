using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CodeHub.Controllers
{
    public class AdminController : Controller
    {
        private readonly BattlesContext _battlesContext;
        private readonly CodeHubDbContext _dbContext;
        private readonly CoursesContext _coursesContext;
        private readonly ExercisesContext _exercisesContext;
        private readonly LessonsContext _lessonsContext;
        private readonly ForumContext _forumContext;
        public AdminController(BattlesContext battlesContext,
            CodeHubDbContext dbContext, CoursesContext coursesContext,
            ExercisesContext exerciseContext, ForumContext forumContext,
            LessonsContext lessonContext)
        {
            _battlesContext = battlesContext;
            _dbContext = dbContext;
            _coursesContext = coursesContext;
            _exercisesContext = exerciseContext;
            _forumContext = forumContext;
            _lessonsContext = lessonContext;
        }
        public async Task<IActionResult> Back()
        {
            return RedirectToAction("Index","Home");
        }
        public async Task<IActionResult> Exercises()
        {
            return PartialView();
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
        public async Task<IActionResult> Lessons()
        {
            return PartialView();
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.ActiveUsers = await _dbContext.Users.CountAsync();
            ViewBag.Battles = await _dbContext.Battles.CountAsync();
            ViewBag.Courses = await _dbContext.Courses.CountAsync();
            ViewBag.ForumPosts = await _dbContext.Forums.CountAsync();

            return PartialView();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllLessons()
        {
            try
            {
                if (_lessonsContext == null) return Ok(new List<Lesson>());
                var lessons = await _lessonsContext.ReadAll(false,true);
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Грешка при извличане на уроци: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("Admin/GetLesson/{id}")]
        public async Task<IActionResult> GetLesson(int id)
        {
            try
            {
                var lesson = await _lessonsContext.Read(id);
                return Ok(lesson);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Грешка при извличане на урок: {ex.Message}" });
            }
        }

        [HttpPost]
        [Route("Admin/CreateLesson")]
        public async Task<IActionResult> CreateLesson([FromForm] string title, [FromForm] string description, [FromForm] IFormFile video)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                    return BadRequest(new { success = false, message = "Името на урока е задължително!" });

                if (video == null)
                    return BadRequest(new { success = false, message = "Видео файлът е задължителен!" });

                if (!video.ContentType.StartsWith("video/"))
                    return BadRequest(new { success = false, message = "Невалиден формат на видео файл!" });

                if (video.Length > 50 * 1024 * 1024)
                    return BadRequest(new { success = false, message = "Видео файлът трябва да е по-малък от 50MB!" });

                byte[] videoBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await video.CopyToAsync(memoryStream);
                    videoBytes = memoryStream.ToArray();
                }

                var lesson = new Lesson
                {
                    Title = title,
                    Description = description,
                    Video = videoBytes
                };

                await _lessonsContext.Create(lesson);
                return Ok(new { success = true, message = "Урокът е създаден успешно!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Грешка при създаване на урок: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("Admin/UpdateLesson/{id}")]
        public async Task<IActionResult> UpdateLesson(int id, [FromForm] string title, [FromForm] string description, [FromForm] IFormFile video)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                    return BadRequest(new { success = false, message = "Името на урока е задължително!" });

                var lesson = await _lessonsContext.Read(id);
                lesson.Title = title;
                lesson.Description = description;

                if (video != null)
                {
                    if (!video.ContentType.StartsWith("video/"))
                        return BadRequest(new { success = false, message = "Невалиден формат на видео файл!" });

                    using (var memoryStream = new MemoryStream())
                    {
                        await video.CopyToAsync(memoryStream);
                        lesson.Video = memoryStream.ToArray();
                    }
                }

                await _lessonsContext.Update(lesson);
                return Ok(new { success = true, message = "Урокът е обновен успешно!" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Грешка при обновяване на урок: {ex.Message}" });
            }
        }

        [HttpDelete]
        [Route("Admin/DeleteLesson/{id}")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            try
            {
                await _lessonsContext.Delete(id);
                return Ok(new { success = true, message = "Урокът е изтрит успешно!" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Грешка при изтриване на урок: {ex.Message}" });
            }
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
                if (battles == null)
                {
                    return Ok(new List<Battle>());
                }
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
                var courses = _coursesContext.ReadAll();
                if (courses == null)
                {
                    return Ok(new List<Course>());
                }
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
                if (course.Lector != null)
                {
                        if (course.Lector.Id <= 0)
                            return BadRequest("Всеки лектор трябва да има валидно ID.");
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
        public async Task<IActionResult> GetAllExercises()
        {
            try
            {
                var exercises = await _dbContext.Exercises.ToListAsync();

                if (exercises == null)
                {
                    return Ok(new List<Exercise>());
                }

                return Ok(exercises);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Грешка при извличане на упражнения: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateExercise([FromBody] Exercise exercise)
        {
            if (exercise == null)
            {
                return BadRequest("Exercise data is required");
            }


            if (string.IsNullOrWhiteSpace(exercise.Title))
            {
                ModelState.AddModelError("Title", "Title is required");
            }

            if (string.IsNullOrWhiteSpace(exercise.Description))
            {
                ModelState.AddModelError("Description", "Description is required");
            }

            if (exercise.Points <= 0)
            {
                ModelState.AddModelError("Points", "Points must be positive");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                exercise.Date = exercise.Date == default ? DateTime.UtcNow : exercise.Date;
                await _exercisesContext.Create(exercise);
                return Ok(new { message = "Exercise created successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating exercise: {ex.Message}");
            }
        }
        [HttpPut]
        [Route("Admin/UpdateExercise")]
        public async Task<IActionResult> UpdateExercise([FromBody] Exercise exercise)
        {
            if (exercise == null)
            {
                return BadRequest("Exercise data is required");
            }

            if (exercise.Id <= 0)
            {
                ModelState.AddModelError("Id", "Valid exercise ID is required");
            }

            if (string.IsNullOrWhiteSpace(exercise.Title))
            {
                ModelState.AddModelError("Title", "Title is required");
            }

            if (string.IsNullOrWhiteSpace(exercise.Description))
            {
                ModelState.AddModelError("Description", "Description is required");
            }

            if (exercise.Points <= 0)
            {
                ModelState.AddModelError("Points", "Points must be positive");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                await _exercisesContext.Update(exercise);

                return Ok(new { message = "Упражнението е обновено успешно!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Грешка при обновяване на упражнение: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        [Route("Admin/DeleteExercise/{id}")]
        public async Task<IActionResult> DeleteExercise(int id)
        {
            try
            {
                var exercise = await _dbContext.Exercises.FindAsync(id);
                if (exercise == null)
                    return NotFound(new { message = "Упражнението не е намерено!" });

                _dbContext.Exercises.Remove(exercise);
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Упражнението е изтрито успешно!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Грешка при изтриване на упражнение: {ex.Message}" });
            }
        }
        [HttpDelete("{id}")]
        [Route("Admin/DeleteBattle/{id}")]
        public async Task<IActionResult> DeleteBattle(int id)
        {
            try
            {
                var battle = await _battlesContext.Read(id);
                if (battle == null)
                    return NotFound(new { message = "Двубоят не е намерен!" });

                await _battlesContext.Delete(id);

                return Ok(new
                {
                    success = true,
                    message = "Двубоят е изтрит успешно!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Грешка при изтриване на двубой: {ex.Message}" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllForumPosts()
        {
            try
            {
                var forumPosts = await _forumContext.ReadAll(true, true);

                return Ok(forumPosts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching forum posts: {ex.Message}");
            }
        }
        [HttpPut]
        [Route("Admin/UpdateBattle/{id}")]
        public async Task<IActionResult> UpdateBattle(int id, [FromBody] Battle battle)
        {
            try
            {
                if (battle == null)
                    return BadRequest("Данните за двубоя са задължителни!");

                if (string.IsNullOrWhiteSpace(battle.Title))
                    return BadRequest("Името на двубоя е задължително!");

                if (battle.StartDate >= battle.EndDate)
                    return BadRequest("Крайната дата трябва да бъде след началната дата!");

                if (battle.RewardXP <= 0)
                    return BadRequest("Наградата трябва да бъде положително число!");


                await _battlesContext.Update(battle);

                return Ok(new { success = true, message = "Двубоят е обновен успешно!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Грешка при обновяване на двубой: {ex.Message}");
            }
        }
        public async Task<IActionResult> Forum_Details(int? id)
        {
            if (id != null)
            {
                var forum = await _forumContext.Read(id.Value, true, true);
                if (forum == null)
                {
                    return NotFound();
                }

                forum.Views++;
                await _forumContext.Update(forum);
                return PartialView(forum);
            }
            return RedirectToAction();
        }
    }
}