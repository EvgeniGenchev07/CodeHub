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
    public class ForumContextTest
    {
        private ForumContext _forumContext;

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
            _forumContext = new ForumContext(Tests.dbContext);
        }

        [Test]
        public async Task CreateForum()
        {
            var user = new User("test@example.com", "Test User");
            var forum = new Forum("Forum Title", "Forum Content", user, new List<Filters>() { Filters.Algorithms });
            int before = Tests.dbContext.Forums.Count();
            await _forumContext.Create(forum);
            int after = Tests.dbContext.Forums.Count();
            var last = Tests.dbContext.Forums.Last();
            Assert.That(before + 1 == after && last.Id == forum.Id, "Forum not created correctly!");
        }

        [Test]
        public async Task ReadForum()
        {
            var user = new User("test@example.com", "Test User");
            var forum = new Forum("Forum Title", "Forum Content", user, new List<Filters>() { Filters.Algorithms });
            await _forumContext.Create(forum);
            var read = await _forumContext.Read(forum.Id);
            Assert.That(read.Title == "Forum Title", "Read() does not get Forum by id!");
        }

        [Test]
        public async Task ReadAllForums()
        {
            int before = Tests.dbContext.Forums.Count();
            var all = await _forumContext.ReadAll();
            Assert.That(before == all.Count, "ReadAll() does not return all Forums!");
        }

        [Test]
        public async Task UpdateForum()
        {
            var user = new User("test@example.com", "Test User");
            var forum = new Forum("Forum Title", "Forum Content", user, new List<Filters>() { Filters.Algorithms });

        await _forumContext.Create(forum);
            var last = (await _forumContext.ReadAll()).Last();
            last.Title = "Updated Forum Title";
            await _forumContext.Update(last);
            Assert.That((await _forumContext.Read(last.Id)).Title == "Updated Forum Title", "Update() does not change the Forum's title!");
        }

        [Test]
        public async Task DeleteForum()
        {
            var user = new User("test@example.com", "Test User");
            var forum = new Forum("Forum Title", "Forum Content", user, new List<Filters>() { Filters.Algorithms });
            await _forumContext.Create(forum);
            var all = await _forumContext.ReadAll();
            int before = all.Count;
            var last = all.Last();
            await _forumContext.Delete(last.Id);
            int after = (await _forumContext.ReadAll()).Count;
            Assert.That(before == after + 1, "Delete() does not delete a Forum!");
        }
    }
} 