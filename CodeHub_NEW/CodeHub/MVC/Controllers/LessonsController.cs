using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVC.Controllers
{
    public class LessonController : Controller
    {
        private readonly LessonsContext _lessonsContext;
        private readonly ExercisesContext _exercisesContext;

        public LessonController(LessonsContext lessonsContext, ExercisesContext exercisesContext)
        {
            _lessonsContext = lessonsContext;
            _exercisesContext = exercisesContext;
        }

        // GET: Lesson
        public IActionResult Index()
        {
            try
            {
                List<Lesson> lessons = _lessonsContext.ReadAll(useNavigationalProperties: true);
                return View(lessons);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving lessons: {ex.Message}";
                return View(new List<Lesson>());
            }
        }

        // GET: Lesson/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Lesson ID not provided";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var lesson = _lessonsContext.Read(id.Value, useNavigationalProperties: true);
                if (lesson == null)
                {
                    TempData["ErrorMessage"] = $"Lesson with ID {id} not found";
                    return RedirectToAction(nameof(Index));
                }
                return View(lesson);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving lesson details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Lesson/Create
        public IActionResult Create()
        {
            ViewBag.AllExercises = _exercisesContext.ReadAll();
            return View();
        }

        // POST: Lesson/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Video,Exercises")] Lesson lesson, List<int> selectedExercises)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Request.Form.Files.Count > 0)
                    {
                        var file = Request.Form.Files[0];
                        if (file.Length > 0)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await file.CopyToAsync(memoryStream);
                                lesson.Video = memoryStream.ToArray();
                            }
                        }
                    }

                    if (selectedExercises != null)
                    {
                        lesson.Exercises = new List<Exercise>();
                        foreach (var exerciseId in selectedExercises)
                        {
                            var exercise = await _exercisesContext.Read(exerciseId);
                            if (exercise != null)
                            {
                                lesson.Exercises.Add(exercise);
                            }
                        }
                    }

                    _lessonsContext.Create(lesson);
                    TempData["SuccessMessage"] = "Lesson created successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating lesson: {ex.Message}");
                }
            }

            ViewBag.AllExercises = _exercisesContext.ReadAll();
            return View(lesson);
        }

        // GET: Lesson/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Lesson ID not provided";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var lesson = _lessonsContext.Read(id.Value, useNavigationalProperties: true);
                if (lesson == null)
                {
                    TempData["ErrorMessage"] = $"Lesson with ID {id} not found";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.AllExercises = _exercisesContext.ReadAll();
                return View(lesson);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving lesson for edit: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Lesson/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Video,Exercises")] Lesson lesson, List<int> selectedExercises)
        {
            if (id != lesson.Id)
            {
                TempData["ErrorMessage"] = "Lesson ID mismatch";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (Request.Form.Files.Count > 0)
                    {
                        var file = Request.Form.Files[0];
                        if (file.Length > 0)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await file.CopyToAsync(memoryStream);
                                lesson.Video = memoryStream.ToArray();
                            }
                        }
                    }
                    else
                    {
                        var existingLesson = _lessonsContext.Read(id);
                        lesson.Video = existingLesson.Video;
                    }

                    if (selectedExercises != null)
                    {
                        lesson.Exercises = new List<Exercise>();
                        foreach (var exerciseId in selectedExercises)
                        {
                            var exercise = await _exercisesContext.Read(exerciseId); // Await the Task to get the Exercise object
                            if (exercise != null)
                            {
                                lesson.Exercises.Add(exercise);
                            }
                        }
                    }

                    _lessonsContext.Update(lesson, useNavigationalProperties: true);
                    TempData["SuccessMessage"] = "Lesson updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating lesson: {ex.Message}");
                }
            }

            ViewBag.AllExercises = _exercisesContext.ReadAll();
            return View(lesson);
        }

        // GET: Lesson/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Lesson ID not provided";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var lesson = _lessonsContext.Read(id.Value, useNavigationalProperties: true);
                if (lesson == null)
                {
                    TempData["ErrorMessage"] = $"Lesson with ID {id} not found";
                    return RedirectToAction(nameof(Index));
                }

                return View(lesson);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving lesson for deletion: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Lesson/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _lessonsContext.Delete(id);
                TempData["SuccessMessage"] = "Lesson deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting lesson: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}