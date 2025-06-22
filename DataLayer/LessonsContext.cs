using BusinessLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class LessonsContext : IDb<Lesson, int>
    {
        CodeHubDbContext dbContext;

        public LessonsContext(CodeHubDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Create(Lesson item)
        {
            dbContext.Lessons.Add(item);
            dbContext.SaveChanges();
        }

        public async Task<Lesson> Read(int key, bool useNavigationalProperties = false, bool isReadOnly = false)
        {
            IQueryable<Lesson> query = dbContext.Lessons;

            if (useNavigationalProperties)
                query = query
                    .Include(e => e.Exercises);
            if (isReadOnly) query = query.AsNoTrackingWithIdentityResolution();

            Lesson lesson = query.FirstOrDefault(l => l.Id == key);

            if (lesson is null) throw new ArgumentException($"Lesson with id = {key} does not exist!");

            return lesson;
        }

        public async Task<List<Lesson>> ReadAll(bool useNavigationalProperties = false, bool isReadOnly = false)
        {
            IQueryable<Lesson> query = dbContext.Lessons;

            if (useNavigationalProperties)
                query = query
                    .Include(e => e.Exercises);
            if (isReadOnly) query = query.AsNoTrackingWithIdentityResolution();

            return query.ToList();
        }

        public async Task Update(Lesson item, bool useNavigationalProperties = false)
        {
            Lesson lessonFromDb = await Read(item.Id, useNavigationalProperties);

            dbContext.Entry<Lesson>(lessonFromDb).CurrentValues.SetValues(item);

            if (useNavigationalProperties)
            {
                List<Exercise> exercises = new List<Exercise>(item.Exercises.Count);
                for (int i = 0; i < item.Exercises.Count; ++i)
                {
                    Exercise exerciseFromDb = dbContext.Exercises.Find(item.Exercises[i].Id);
                    if (exerciseFromDb != null) exercises.Add(exerciseFromDb);
                    else exercises.Add(item.Exercises[i]);
                }

                lessonFromDb.Exercises = exercises;
            }

            dbContext.SaveChanges();
        }

        public async Task Delete(int key)
        {
            Lesson lesson = await Read(key);
            dbContext.Lessons.Remove(lesson);
            dbContext.SaveChanges();
        }
    }
}
