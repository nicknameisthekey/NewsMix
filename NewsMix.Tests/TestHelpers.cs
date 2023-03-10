using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NewsMix.Services;

public static partial class TestHelpers
{
    public static UserService NewUserService => new UserService(CreateDb().repo);

    public static (SqliteRepository repo, SqliteContext ctx) CreateDb()
    {
        var optionsBuilder = new DbContextOptionsBuilder<SqliteContext>();
        var file = Path.Combine(TestHelpers.fileStoragePath + "test.db");
        optionsBuilder.UseSqlite($"Data Source={file}");
        var ctx = new SqliteContext(optionsBuilder.Options);
        ctx.Database.EnsureDeleted();
        ctx.Database.EnsureCreated();
        var repo = new SqliteRepository(ctx);
        return (repo, ctx);
    }

    public static readonly string fileStoragePath = Path.Combine(Assembly.GetExecutingAssembly()
                             .Location.Replace("NewsMix.Tests.dll", ""), "file_repo_tests");
}