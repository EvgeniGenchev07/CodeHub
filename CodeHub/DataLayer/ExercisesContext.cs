using BusinessLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class ExercisesContext
    {
        ApplicationDbContext dbContext;

        public ExercisesContext(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Create(Exercise item)
        {
            dbContext.Exercises.Add(item);
            dbContext.SaveChanges();
        }

        public Exercise Read(int key, bool isReadOnly = false)
        {
            IQueryable<Exercise> query = dbContext.Exercises;

            if (isReadOnly) query = query.AsNoTrackingWithIdentityResolution();

            Exercise exercise = query.FirstOrDefault(r => r.Id == key);

            if (exercise == null) throw new ArgumentException($"Exercise with id = {key} does not exist!");

            return exercise;
        }

        public List<Exercise> ReadAll(bool isReadOnly = false)
        {
            IQueryable<Exercise> query = dbContext.Exercises;

            if (isReadOnly) query = query.AsNoTrackingWithIdentityResolution();

            return query.ToList();
        }

        public void Update(Exercise item)
        {
            Exercise exerciseFromDb = Read(item.Id);

            dbContext.Entry<Exercise>(exerciseFromDb).CurrentValues.SetValues(item);

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
