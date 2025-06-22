using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer;
using DataLayer;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace TestLayer
{
    [TestFixture]
    public class LessonsContextTest
    {
        private LessonsContext _lessonsContext;

        [SetUp]
        public void Setup()
        {
            if (Tests.dbContext == null)
            {
                
                var options = new DbContextOptionsBuilder<CodeHubDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
                Tests.dbContext = new CodeHubDbContext(options);
            }
            _lessonsContext = new LessonsContext(Tests.dbContext);
        }

        [Test]
        public async Task CreateLesson()
        {
            var lesson = new Lesson("Lesson Title", "Lesson Description", new byte[] { 1, 2, 3 });
            int before = Tests.dbContext.Lessons.Count();
            await _lessonsContext.Create(lesson);
            int after = Tests.dbContext.Lessons.Count();
            var last = Tests.dbContext.Lessons.Last();
            Assert.That(before + 1 == after && last.Id == lesson.Id, "Lesson not created correctly!");
        }

        [Test]
        public async Task ReadLesson()
        {
            var lesson = new Lesson("Lesson Title", "Lesson Description", new byte[] { 1, 2, 3 });
            await _lessonsContext.Create(lesson);
            var read = await _lessonsContext.Read(lesson.Id);
            Assert.That(read.Title == "Lesson Title", "Read() does not get Lesson by id!");
        }

        [Test]
        public async Task ReadAllLessons()
        {
            int before = Tests.dbContext.Lessons.Count();
            var all = await _lessonsContext.ReadAll();
            Assert.That(before == all.Count, "ReadAll() does not return all Lessons!");
        }

        [Test]
        public async Task UpdateLesson()
        {
            var lesson = new Lesson("Lesson Title", "Lesson Description", new byte[] { 1, 2, 3 });
            await _lessonsContext.Create(lesson);
            var last = (await _lessonsContext.ReadAll()).Last();
            last.Title = "Updated Lesson Title";
            await _lessonsContext.Update(last);
            Assert.That((await _lessonsContext.Read(last.Id)).Title == "Updated Lesson Title", "Update() does not change the Lesson's title!");
        }

        [Test]
        public async Task DeleteLesson()
        {
            var lesson = new Lesson("Lesson Title", "Lesson Description", new byte[] { 1, 2, 3 });
            await _lessonsContext.Create(lesson);
            var all = await _lessonsContext.ReadAll();
            int before = all.Count;
            var last = all.Last();
            await _lessonsContext.Delete(last.Id);
            int after = (await _lessonsContext.ReadAll()).Count;
            Assert.That(before == after + 1, "Delete() does not delete a Lesson!");
        }
    }
} 