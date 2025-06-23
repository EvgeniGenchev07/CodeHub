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
            await dbContext.Lessons.AddAsync(item);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Lesson> Read(int key, bool useNavigationalProperties = false, bool isReadOnly = false)
        {
            IQueryable<Lesson> query = dbContext.Lessons;

            if (isReadOnly) query = query.AsNoTrackingWithIdentityResolution();

            Lesson lesson = query.FirstOrDefault(l => l.Id == key);

            if (lesson is null) throw new ArgumentException($"Lesson with id = {key} does not exist!");

            return lesson;
        }

        public async Task<List<Lesson>> ReadAll(bool useNavigationalProperties = false, bool isReadOnly = false)
        {
            IQueryable<Lesson> query = dbContext.Lessons;


            if (isReadOnly) query = query.AsNoTrackingWithIdentityResolution();

            return query.ToList();
        }

        public async Task Update(Lesson item, bool useNavigationalProperties = false)
        {
            Lesson lessonFromDb = await Read(item.Id, useNavigationalProperties);

            dbContext.Entry<Lesson>(lessonFromDb).CurrentValues.SetValues(item);

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
