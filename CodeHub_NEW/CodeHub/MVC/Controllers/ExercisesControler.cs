using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace MVC.Controllers
{
    public class ExercisesController : Controller
    {
        private readonly ExercisesContext _exercisesContext;

        public ExercisesController(ExercisesContext exercisesContext)
        {
            _exercisesContext = exercisesContext;
        }

        // GET: Exercise
        public IActionResult Index()
        {
            try
            {
                List<Exercise> exercises = _exercisesContext.ReadAll();
                return View(exercises);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving exercises: {ex.Message}";
                return View(new List<Exercise>());
            }
        }

        // GET: Exercise/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Exercise ID not provided";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var exercise = _exercisesContext.Read(id.Value);
                if (exercise == null)
                {
                    TempData["ErrorMessage"] = $"Exercise with ID {id} not found";
                    return RedirectToAction(nameof(Index));
                }
                return View(exercise);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving exercise details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Exercise/Create
        public IActionResult Create()
        {
            ViewBag.DifficultyLevels = Enum.GetValues(typeof(Difficulty));
            return View();
        }

        // POST: Exercise/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Title,Description,Points,Difficulty")] Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _exercisesContext.Create(exercise);
                    TempData["SuccessMessage"] = "Exercise created successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating exercise: {ex.Message}");
                }
            }

            ViewBag.DifficultyLevels = Enum.GetValues(typeof(Difficulty));
            return View(exercise);
        }

        // GET: Exercise/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Exercise ID not provided";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var exercise = _exercisesContext.Read(id.Value);
                if (exercise == null)
                {
                    TempData["ErrorMessage"] = $"Exercise with ID {id} not found";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.DifficultyLevels = Enum.GetValues(typeof(Difficulty));
                return View(exercise);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving exercise for edit: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Exercise/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Title,Description,Points,Difficulty")] Exercise exercise)
        {
            if (id != exercise.Id)
            {
                TempData["ErrorMessage"] = "Exercise ID mismatch";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _exercisesContext.Update(exercise);
                    TempData["SuccessMessage"] = "Exercise updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating exercise: {ex.Message}");
                }
            }

            ViewBag.DifficultyLevels = Enum.GetValues(typeof(Difficulty));
            return View(exercise);
        }

        // GET: Exercise/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Exercise ID not provided";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var exercise = _exercisesContext.Read(id.Value);
                if (exercise == null)
                {
                    TempData["ErrorMessage"] = $"Exercise with ID {id} not found";
                    return RedirectToAction(nameof(Index));
                }

                return View(exercise);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving exercise for deletion: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Exercise/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _exercisesContext.Delete(id);
                TempData["SuccessMessage"] = "Exercise deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting exercise: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}