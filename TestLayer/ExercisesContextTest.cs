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
    public class ExercisesContextTest
    {
        private ExercisesContext _exercisesContext;

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
            _exercisesContext = new ExercisesContext(Tests.dbContext);
        }

        [Test]
        public async Task CreateExercise()
        {
            var exercise = new Exercise("Title", "Description", 10, Difficulty.Easy);
            exercise.Date = DateTime.Now;
            int before = Tests.dbContext.Exercises.Count();
            await _exercisesContext.Create(exercise);
            int after = Tests.dbContext.Exercises.Count();
            var last = Tests.dbContext.Exercises.Last();
            Assert.That(before + 1 == after && last.Id == exercise.Id, "Exercise not created correctly!");
        }

        [Test]
        public async Task ReadExercise()
        {
            var exercise = new Exercise("Title", "Description", 10, Difficulty.Easy);
            exercise.Date = DateTime.Now;
            await _exercisesContext.Create(exercise);
            var read = await _exercisesContext.Read(exercise.Id);
            Assert.That(read.Title == "Title", "Read() does not get Exercise by id!");
        }

        [Test]
        public async Task ReadAllExercises()
        {
            int before = Tests.dbContext.Exercises.Count();
            var all = await _exercisesContext.ReadAll();
            Assert.That(before == all.Count, "ReadAll() does not return all Exercises!");
        }

        [Test]
        public async Task UpdateExercise()
        {
            var exercise = new Exercise("Title", "Description", 10, Difficulty.Easy);
            exercise.Date = DateTime.Now;
            await _exercisesContext.Create(exercise);
            var last = (await _exercisesContext.ReadAll()).Last();
            last.Title = "Updated Title";
            await _exercisesContext.Update(last);
            Assert.That((await _exercisesContext.Read(last.Id)).Title == "Updated Title", "Update() does not change the Exercise's title!");
        }

        [Test]
        public async Task DeleteExercise()
        {
            var exercise = new Exercise("Title", "Description", 10, Difficulty.Easy);
            exercise.Date = DateTime.Now;
            await _exercisesContext.Create(exercise);
            var all = await _exercisesContext.ReadAll();
            int before = all.Count;
            var last = all.Last();
            await _exercisesContext.Delete(last.Id);
            int after = (await _exercisesContext.ReadAll()).Count;
            Assert.That(before == after + 1, "Delete() does not delete an Exercise!");
        }
    }
} 