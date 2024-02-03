using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsMix.Services;
using NewsMix.Storage;

namespace NewsMix.Tests;

public static class TestHelpers
{
    public static void ReplaceWithSingleton<T>(this IServiceCollection sc, T newObj) where T : class
    {
        sc.Remove(sc.First(d => d.ServiceType == typeof(T)));
        sc.AddSingleton<T>(newObj);
    }

    public static IConfiguration MockConfig => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            { "sqlite", "sqlite" },
        }!)
        .Build();

    public static UserService NewUserService => new(CreateDb().repo);

    public static (SqliteRepository repo, SqliteContext ctx) CreateDb()
    {
        var optionsBuilder = new DbContextOptionsBuilder<SqliteContext>();
        var file = Path.Combine(fileStoragePath + "test.db");
        optionsBuilder.UseSqlite($"Data Source={file}");
        var ctx = new SqliteContext(optionsBuilder.Options);
        ctx.Database.EnsureDeleted();
        ctx.Database.EnsureCreated();
        var repo = new SqliteRepository(ctx);
        return (repo, ctx);
    }

    private static readonly string fileStoragePath = Path.Combine(Assembly.GetExecutingAssembly()
        .Location.Replace("NewsMix.Tests.dll", ""), "file_repo_tests");
}