using BusinessLayer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataLayer;

public class ApplicationDbContext : IdentityDbContext<User>
{
    internal DbSet<Course> Courses { get; set; }
    internal DbSet<Exercise> Exercises { get; set; }
    internal DbSet<Lector> Lectors { get; set; }
    internal DbSet<Lesson> Lessons { get; set; }
    public ApplicationDbContext() : base()
    {
        
    }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
    {
        
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) optionsBuilder.UseSqlite("DataSource=codehub.db3;Cache=Shared");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<User>(u =>
        {
            u.Property(p=>p.FullName).IsRequired();
        });
    }
}
