using BusinessLayer;
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
            dbContext.SaveChanges();
        }

        public async Task<Course> Read(int key, bool useNavigationalProperties = false, bool isReadOnly = false)
        {
            IQueryable<Course> query = dbContext.Courses;

            if (useNavigationalProperties)
                query = query
                    .Include(l => l.Lectors)
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
                    .Include(l => l.Lectors)
                    .Include(l => l.Lessons);
            if (isReadOnly) query = query.AsNoTrackingWithIdentityResolution();

            return query.ToList();
        }

        public async Task Update(Course item, bool useNavigationalProperties = false)
        {
            Course courseFromDb = Read(item.Id, useNavigationalProperties);

            dbContext.Entry<Course>(courseFromDb).CurrentValues.SetValues(item);

            if (useNavigationalProperties)
            {
                List<Lector> lectors = new List<Lector>(item.Lectors.Count);
                for (int i = 0; i < item.Lectors.Count; ++i)
                {
                    Lector lectorFromDb = dbContext.Lectors.Find(item.Lectors[i].Id);
                    if (lectorFromDb != null) lectors.Add(lectorFromDb);
                    else lectors.Add(item.Lectors[i]);
                }

                courseFromDb.Lectors = lectors;

                List<Lesson> lessons = new List<Lesson>(item.Lessons.Count);
                for (int i = 0; i < item.Lessons.Count; ++i)
                {
                    Lesson lessonFromDb = dbContext.Lessons.Find(item.Lessons[i].Id);
                    if (lessonFromDb != null) lessons.Add(lessonFromDb);
                    else lessons.Add(item.Lessons[i]);
                }

                courseFromDb.Lessons = lessons;
            }

            dbContext.SaveChanges();
        }

        public async Task Delete(int key)
        {
            Course course = Read(key);
            dbContext.Courses.Remove(course);
            dbContext.SaveChanges();
        }
    }
}
