using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Tests.Infrastructure.Repositories;

public class BaseRepositoryTests
{
    internal readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public BaseRepositoryTests(string databaseName)
        => _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

    public virtual ApplicationDbContext CreateContext()
        => new(_dbContextOptions);
}