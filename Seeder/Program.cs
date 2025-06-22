﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BusinessLayer;
using DataLayer;

try
{
    IdentityOptions options = new IdentityOptions();
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 5;

    DbContextOptionsBuilder<CodeHubDbContext> builder = new DbContextOptionsBuilder<CodeHubDbContext>();
    builder.UseSqlite("DataSource=../../../../DataLayer/codehub.db3");

    CodeHubDbContext dbContext = new CodeHubDbContext(builder.Options);
    UserManager<User> userManager = new UserManager<User>(
        new UserStore<User>(dbContext), Options.Create(options),
        new PasswordHasher<User>(), new List<IUserValidator<User>>() { new UserValidator<User>() },
        new List<IPasswordValidator<User>>(), new UpperInvariantLookupNormalizer(),
        new IdentityErrorDescriber(), new ServiceCollection().BuildServiceProvider(),
        new Logger<UserManager<User>>(new LoggerFactory())
    );

    IdentityContext identityContext = new IdentityContext(dbContext, userManager);

    dbContext.Roles.Add(new IdentityRole("Administrator") { NormalizedName = "ADMINISTRATOR" });
    dbContext.Roles.Add(new IdentityRole("User") { NormalizedName = "USER" });
    await dbContext.SaveChangesAsync();
    Console.WriteLine("Roles added succssfully!");
    var user = new User( "admincho@abv.bg","admin");
    user.ProfilePicture = new byte[] { 2, 3, 3, 4, 2, 55, 6, 4, 7 };
    await identityContext.CreateUserAsync(user, "admin", Role.ADMINISTRATOR);

    Console.WriteLine("Admin account added successfully!");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
