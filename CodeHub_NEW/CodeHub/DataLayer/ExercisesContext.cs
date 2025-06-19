using BusinessLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataLayer
{
    public class ExercisesContext : IDb<Exercise, int>
    {
        private readonly CodeHubDbContext _dbContext;

        public ExercisesContext(CodeHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Create(Exercise item)
        {
            try
            {
                await _dbContext.Exercises.AddAsync(item);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Failed to create exercise. Possible duplicate or invalid data.", ex);
            }
        }

        public async Task<Exercise> Read(int key, bool useNavigationalProperties = false, bool isReadOnly = false)
        {
            IQueryable<Exercise> query = _dbContext.Exercises;

            if (isReadOnly)
            {
                query = query.AsNoTracking();
            }

            var exercise = await query.FirstOrDefaultAsync(e => e.Id == key);

            return exercise ?? throw new KeyNotFoundException($"Exercise with id {key} not found");
        }

        public async Task<List<Exercise>> ReadAll(bool useNavigationalProperties = false, bool isReadOnly = false)
        {
            IQueryable<Exercise> query = _dbContext.Exercises;

            if (isReadOnly)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        public async Task Update(Exercise item, bool useNavigationalProperties = false)
        {
            try
            {
                var existingExercise = await _dbContext.Exercises.FindAsync(item.Id);
                if (existingExercise == null)
                    throw new KeyNotFoundException($"Exercise with id {item.Id} not found");
                existingExercise.Title = item.Title;
                existingExercise.Description = item.Description;
                existingExercise.Points = item.Points;
                existingExercise.Views = item.Views;
                existingExercise.Solutions = item.Solutions;
                existingExercise.Date = item.Date;
                existingExercise.Difficulty = item.Difficulty;

                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Exercise was modified or deleted by another user.", ex);
            }
        }


        public async Task Delete(int key)
        {
            var exercise = await Read(key);
            _dbContext.Exercises.Remove(exercise);
            await _dbContext.SaveChangesAsync();
        }
    }
}
