using BusinessLayer;
using DataLayer;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class CoursesController : Controller
    {
        private readonly CoursesContext _coursesContext;
        private readonly IdentityContext _context;
        private const int pageSize = 10;
        public CoursesController(CoursesContext coursesContext,IdentityContext identityContext)
        {
            _coursesContext = coursesContext;
            _context = identityContext;
        }

        // GET: Course
        public async Task<IActionResult> Index([FromQuery] int page = 1, [FromQuery] string search = null, [FromQuery]Filters? filter = null, [FromQuery] Difficulty? level = null)
        {
            try
            {
                //var allCourses = await _coursesContext.ReadAll(useNavigationalProperties: true);
                var allCourses = new List<Course>()
                {
                    new Course()
                    {
                        Filters = new List<Filters>() { Filters.Algorithms },
                        Description = "dasdad",
                        Name = "dasdad",
                        Lector= 
                            new Lector()
                            {
                                Name = "Adada",
                                Description = "dasdad"
                            },
                        Lessons = new List<Lesson>()
                        {
                            new Lesson()
                            {
                                Description = "dasdad",
                                Title = "dasdad",
                                Video = new byte[]{1,3,4,5,6,5,7}
                            }
                        },
                        Difficulty = Difficulty.Easy,
                    }
                };
                // Filtering
                if (!string.IsNullOrWhiteSpace(search))
                    allCourses = allCourses.FindAll(c => c.Name.Contains(search, StringComparison.OrdinalIgnoreCase) || c.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
                if (level.HasValue && level.Value != 0)
                    allCourses = allCourses.FindAll(c => c.Difficulty == level);
                if (filter.HasValue && filter.Value != 0)
                    allCourses = allCourses.FindAll(c=>c.Filters.Any(f=>f == filter));
                // Pagination
                int totalCourses = allCourses.Count;
                int totalPages = (int)Math.Ceiling(totalCourses / (double)pageSize);
                allCourses = allCourses.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                ViewBag.Page = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.Filter = filter;
                ViewBag.Search = search;
                ViewBag.Level = level;
                return View(allCourses);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving courses: {ex.Message}";
                return View(new List<Course>());
            }
        }
        // GET: Course/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Course ID not provided";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                //var course = await _coursesContext.Read(id.Value, useNavigationalProperties: true);
                   var course =  new Course()
                    {
                        Filters = new List<Filters>() { Filters.Algorithms },
                        Description = "dasdad",
                        Name = "dasdad",
                        Lector = 
                            new Lector()
                            {
                                Name = "Adada",
                                Description = "dasdad"
                            },
                        Lessons = new List<Lesson>()
                        {
                            new Lesson()
                            {
                                Description = "dasdad",
                                Title = "dasdad",
                                Video = new byte[]{1,3,4,5,6,5,7}
                            }
                        },
                        Difficulty = Difficulty.Easy,
                    };
                    if (User.Identity.IsAuthenticated)
                    {
                        User user = await _context.ReadUserAsync(User.Identity.GetUserId());
                        if (user != null)
                        {
                            UserCourse userCourse = user.Courses.FirstOrDefault(course => course.Id == id);
                            return View(userCourse);
                        }
                    }
                if (course == null)
                {
                    TempData["ErrorMessage"] = $"Course with ID {id} not found";
                    return RedirectToAction(nameof(Index));
                }
                return View(course);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving course details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Details(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                User user = await _context.ReadUserAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    Course course = await _coursesContext.Read(id);
                    UserCourse userCourse = new UserCourse() { Course = course, User = user,Completion = 0};
                    user.Courses.Add(userCourse);
                    course.Students++;
                    await _context.Save();
                    return View(userCourse);
                }
            }
            return View(_coursesContext.Read(id).Result);
        }
        // GET: Course/Create
        public IActionResult Create()
        {
            ViewBag.DifficultyLevels = Enum.GetValues(typeof(Difficulty));
            return View();
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Difficulty")] Course course)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _coursesContext.Create(course);
                    TempData["SuccessMessage"] = "Course created successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating course: {ex.Message}");
                }
            }
            ViewBag.DifficultyLevels = Enum.GetValues(typeof(Difficulty));
            return View(course);
        }

        // GET: Course/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Course ID not provided";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                var course = await _coursesContext.Read(id.Value, useNavigationalProperties: true);
                if (course == null)
                {
                    TempData["ErrorMessage"] = $"Course with ID {id} not found";
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.DifficultyLevels = Enum.GetValues(typeof(Difficulty));
                return View(course);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving course for edit: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Difficulty,Lectors,Lessons")] Course course)
        {
            if (id != course.Id)
            {
                TempData["ErrorMessage"] = "Course ID mismatch";
                return RedirectToAction(nameof(Index));
            }
            if (ModelState.IsValid)
            {
                try
                {
                    await _coursesContext.Update(course, useNavigationalProperties: true);
                    TempData["SuccessMessage"] = "Course updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating course: {ex.Message}");
                }
            }
            ViewBag.DifficultyLevels = Enum.GetValues(typeof(Difficulty));
            return View(course);
        }

        public async Task<IActionResult> Video(int? courseId,int? id)
        {
            if (courseId != null && id != null)
            {
                return Ok(new byte[]{1,2,3,4,5,6});
                Course corse =  await _coursesContext.Read(courseId.Value, useNavigationalProperties: true,true);
                Lesson lesson = corse.Lessons.FirstOrDefault(l => l.Id == id);
                if (lesson != null)
                {
                    return Ok(lesson.Video);
                }
            }
            return BadRequest();
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> VideoCompletion(int? id)
        {
            if (id != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user =  await _context.ReadUserAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        user.Courses.ForEach(uc=>
                        {
                            if (uc.Course.Id==id)
                            {
                                uc.Completion += 1*100/uc.Course.Lessons.Count;
                                _context.Save();
                                return;
                            }
                        });
                    }
                }
            }
            return BadRequest();
        }
        
        // GET: Course/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Course ID not provided";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                var course = await _coursesContext.Read(id.Value, useNavigationalProperties: true);
                if (course == null)
                {
                    TempData["ErrorMessage"] = $"Course with ID {id} not found";
                    return RedirectToAction(nameof(Index));
                }
                return View(course);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving course for deletion: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _coursesContext.Delete(id);
                TempData["SuccessMessage"] = "Course deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting course: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
