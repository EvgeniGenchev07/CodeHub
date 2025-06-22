using BusinessLayer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class CodeHubDbContext:IdentityDbContext<User>
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Lector> Lectors { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }  
        public CodeHubDbContext() : base()
        {
            
        }
        public CodeHubDbContext(DbContextOptions<CodeHubDbContext> options) : base(options)
        {
           
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Forum>(f =>
            {
                f.Property(p=>p.Date).IsRequired();
                f.HasOne(p=>p.Author).WithMany(u=>u.Forums);
            });
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=codehub.db3");
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}

