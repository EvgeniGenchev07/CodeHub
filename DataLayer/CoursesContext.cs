﻿using BusinessLayer;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class CoursesContext : IDb<Course, int>
    {
        CodeHubDbContext dbContext;

        public CoursesContext(CodeHubDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Create(Course item)
        {
            dbContext.Courses.Add(item);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Course> Read(int key, bool useNavigationalProperties = false, bool isReadOnly = false)
        {
            IQueryable<Course> query = dbContext.Courses;

            if (useNavigationalProperties)
                query = query
                    .Include(l => l.Lector)
                    .Include(l => l.Lessons);
            if (isReadOnly) query = query.AsNoTrackingWithIdentityResolution();

            Course course = query.FirstOrDefault(c => c.Id == key);

            if (course is null) throw new ArgumentException($"Course with id = {key} does not exist!");

            return course;
        }

        public async Task<List<Course>> ReadAll(bool useNavigationalProperties = false, bool isReadOnly = false)
        {
            IQueryable<Course> query = dbContext.Courses;

            if (useNavigationalProperties)
                query = query
                    .Include(l => l.Lector)
                    .Include(l => l.Lessons);
            if (isReadOnly) query = query.AsNoTrackingWithIdentityResolution();

            return query.ToList();
        }

        public async Task Update(Course item, bool useNavigationalProperties = false)
        {
            Course courseFromDb = await Read(item.Id, useNavigationalProperties);

            dbContext.Entry<Course>(courseFromDb).CurrentValues.SetValues(item);

            if (useNavigationalProperties)
            {
                var lector = dbContext.Lectors.FirstOrDefault(l=>l.Id==item.Lector.Id);
                if (lector != null)
                courseFromDb.Lector = lector;

                List<Lesson> lessons = new List<Lesson>(item.Lessons.Count);
                for (int i = 0; i < item.Lessons.Count; ++i)
                {
                    Lesson lessonFromDb = dbContext.Lessons.Find(item.Lessons[i].Id);
                    if (lessonFromDb != null) lessons.Add(lessonFromDb);
                    else lessons.Add(item.Lessons[i]);
                }

                courseFromDb.Lessons = lessons;
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(int key)
        {
            Course course = await Read(key);
            dbContext.Courses.Remove(course);
            await dbContext.SaveChangesAsync();
        }
    }
}
