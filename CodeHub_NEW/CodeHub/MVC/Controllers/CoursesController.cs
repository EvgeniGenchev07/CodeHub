using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVC.Controllers
{
    public class CoursesController : Controller
    {
        private readonly CoursesContext _coursesContext;

        public CoursesController(CoursesContext coursesContext)
        {
            _coursesContext = coursesContext;
        }

        // GET: Course
        public IActionResult Index([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = null, [FromQuery] string sort = null, [FromQuery] string order = null,[FromQuery] Difficulty level = 0)
        {
            try
            {
                List<Course> courses = _coursesContext.ReadAll(useNavigationalProperties: true);
                return View(courses);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving courses: {ex.Message}";
                return View(new List<Course>());
            }
        }
        //This is only for test the design
        public IActionResult Details()
        {
            return View();
        }
        /*
        // GET: Course/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Course ID not provided";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var course = _coursesContext.Read(id.Value, useNavigationalProperties: true);
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
        }*/

        // GET: Course/Create
        public IActionResult Create()
        {
            ViewBag.DifficultyLevels = Enum.GetValues(typeof(Difficulty));
            return View();
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,Description,Difficulty")] Course course)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _coursesContext.Create(course);
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
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Course ID not provided";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var course = _coursesContext.Read(id.Value, useNavigationalProperties: true);
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
        public IActionResult Edit(int id, [Bind("Id,Name,Description,Difficulty,Lectors,Lessons")] Course course)
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
                    _coursesContext.Update(course, useNavigationalProperties: true);
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

        // GET: Course/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Course ID not provided";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var course = _coursesContext.Read(id.Value, useNavigationalProperties: true);
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
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _coursesContext.Delete(id);
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
