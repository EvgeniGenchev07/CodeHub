using DataLayer;
using Microsoft.EntityFrameworkCore;

namespace TestLayer;

public class Tests
{
    internal static CodeHubDbContext dbContext;
    [OneTimeSetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<CodeHubDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        dbContext = new CodeHubDbContext(options);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        dbContext.Dispose();
    }
}