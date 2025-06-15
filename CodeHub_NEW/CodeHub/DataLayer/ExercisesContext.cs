using BusinessLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataLayer
{
    public class ExercisesContext : IDb<Exercise, int>
    {
        private readonly CodeHubDbContext dbContext;

        public ExercisesContext(CodeHubDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Create(Exercise item)
        {
            dbContext.Exercises.Add(item);
            dbContext.SaveChanges();
        }

        public Exercise Read(int key, bool useNavigationalProperties = false, bool isReadOnly = false)
        {
            IQueryable<Exercise> query = dbContext.Exercises;

            // Future enhancement: useNavigationalProperties to include related entities
            if (isReadOnly)
                query = query.AsNoTrackingWithIdentityResolution();

            Exercise exercise = query.FirstOrDefault(e => e.Id == key);

            if (exercise == null)
                throw new ArgumentException($"Exercise with id = {key} does not exist!");

            return exercise;
        }

        public List<Exercise> ReadAll(bool useNavigationalProperties = false, bool isReadOnly = false)
        {
            IQueryable<Exercise> query = dbContext.Exercises;

            // Future enhancement: useNavigationalProperties to include related entities
            if (isReadOnly)
                query = query.AsNoTrackingWithIdentityResolution();

            return query.ToList();
        }

        public void Update(Exercise item, bool useNavigationalProperties = false)
        {
            Exercise exerciseFromDb = Read(item.Id, useNavigationalProperties);

            dbContext.Entry(exerciseFromDb).CurrentValues.SetValues(item);

            dbContext.SaveChanges();
        }

        public void Delete(int key)
        {
            Exercise exerciseFromDb = Read(key);
            dbContext.Exercises.Remove(exerciseFromDb);
            dbContext.SaveChanges();
        }
    }
}
