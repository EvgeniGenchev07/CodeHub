using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BusinessLayer;
using DataLayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeedingDB
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                IdentityOptions options = new IdentityOptions();
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 5;

                DbContextOptionsBuilder<ApplicationDbContext> builder = new DbContextOptionsBuilder<ApplicationDbContext>();
                builder.UseSqlite("DataSource=../DataLayer/codehub.db3;Cache=Shared");
                
                ApplicationDbContext dbContext = new ApplicationDbContext(builder.Options);
                UserManager<User> userManager = new UserManager<User>(
                    new UserStore<User>(dbContext), Options.Create(options),
                    new PasswordHasher<User>(), new List<IUserValidator<User>>() { new UserValidator<User>() },
                    new List<IPasswordValidator<User>>(), new UpperInvariantLookupNormalizer(),
                    new IdentityErrorDescriber(), new ServiceCollection().BuildServiceProvider(),
                    new Logger<UserManager<User>>(new LoggerFactory())
                    );

                IdentityContext identityContext = new IdentityContext(dbContext, userManager);
                
                dbContext.Roles.Add(new IdentityRole("Administrator") { NormalizedName = "ADMINISTRATOR"});
                dbContext.Roles.Add(new IdentityRole("User") { NormalizedName = "USER" });
                await dbContext.SaveChangesAsync();
                Console.WriteLine("Roles added succssfully!");
                await identityContext.CreateUserAsync("admin", "admin", "admincho@abv.bg", Role.ADMINISTRATOR);

                Console.WriteLine("Admin account added successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
