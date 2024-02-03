using Microsoft.EntityFrameworkCore;
using NewsMix.Storage.Entites;

namespace NewsMix.Storage;

public class SqliteContext : DbContext
{
    public SqliteContext(DbContextOptions<SqliteContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserAction> UserActions { get; set; } = null!;
    public DbSet<NotifiedPublication> NotifiedPublications { get; set; } = null!;
}