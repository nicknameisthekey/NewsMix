using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NewsMix.Storage;

namespace NewsMix.Tests;

public static class TestHelpers
{
    public static IServiceCollection WithSingletonMock<T>(this IServiceCollection sc, T newObj) where T : class
    {
        sc.RemoveAll(typeof(T));
        sc.AddSingleton<T>(newObj);
        return sc;
    }

    public static IServiceCollection WithScopedMock<T>(this IServiceCollection sc, T newObj) where T : class
    {
        sc.RemoveAll(typeof(T));
        sc.AddScoped<T>(_ => newObj);
        return sc;
    }

    public static IServiceCollection WithScopedMock<I, T>(this IServiceCollection sc, T newObj) where T : class, I 
                                                                                                where I: class
    {
        sc.RemoveAll(typeof(I));
        sc.AddScoped<I, T>(_ => newObj);
        return sc;
    }

    public static IServiceProvider CreateScope(this IServiceCollection sc)
    {
        return sc.BuildServiceProvider()
            .CreateScope()
            .ServiceProvider;
    }

    public static T GetHostedService<T>(this IServiceProvider sp) where T : class
    {
       return (T)sp.GetRequiredService<IEnumerable<IHostedService>>()
            .SingleOrDefault(h => h.GetType() == typeof(T))!;
    }
    
    public static IConfiguration PrepareTestEnv()
    {
        var optionsBuilder = new DbContextOptionsBuilder<SqliteContext>();
        var file = Path.Combine(Assembly.GetExecutingAssembly()
            .Location.Replace("NewsMix.Tests.dll", ""), "test.db");
        var connString = $"Data Source={file}";
        optionsBuilder.UseSqlite(connString);
        var ctx = new SqliteContext(optionsBuilder.Options);
        ctx.Database.EnsureDeleted();
        ctx.Database.EnsureCreated();

        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:sqlite", connString },
            }!)
            .Build();
    }
}