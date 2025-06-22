﻿using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace MVC.Controllers
{
    public class ExercisesController : Controller
    {
        private readonly ExercisesContext _exercisesContext;
        private const int exercisePageSize = 10;
        public ExercisesController(ExercisesContext exercisesContext)
        {
            _exercisesContext = exercisesContext;
        }

        // GET: Exercise
        public async Task<IActionResult> Index(
         [FromQuery] int page = 1,
         [FromQuery] Difficulty difficulty = Difficulty.All,
         [FromQuery] string search = null,
         [FromQuery] SpecialFilter type = SpecialFilter.All)
        {
            try
            {
                var exercises = (await _exercisesContext.ReadAll(false, true)).ToList();

                // Apply type filter
                exercises = type switch
                {
                    SpecialFilter.New => exercises.OrderByDescending(e => e.Date).Take(10).ToList(),
                    SpecialFilter.Popular => exercises.Where(e => e.Views >= 100)
                                                    .OrderByDescending(e => e.Views)
                                                    .ToList(),
                    _ => exercises
                };

                // Apply difficulty filter
                if (difficulty != Difficulty.All)
                {
                    exercises = exercises.Where(e => e.Difficulty == difficulty).ToList();
                }

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    exercises = exercises.Where(e =>
                        e.Title.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // Calculate pagination
                var totalCount = exercises.Count;
                var totalPages = (int)Math.Ceiling(totalCount / (double)exercisePageSize);
                page = Math.Clamp(page, 1, totalPages == 0 ? 1 : totalPages);

                ViewBag.TotalPages = totalPages;
                ViewBag.Page = page;
                ViewBag.Difficulty = difficulty;
                ViewBag.Search = search;
                ViewBag.Type = type;

                // Apply pagination
                var pagedExercises = exercises
                    .Skip((page - 1) * exercisePageSize)
                    .Take(exercisePageSize)
                    .ToList();

                return View(pagedExercises);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving exercises: {ex.Message}";
                return View(new List<Exercise>());
            }
        }

        // GET: Exercise/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Exercise ID not provided";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var exercise = await _exercisesContext.Read(id.Value);
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
