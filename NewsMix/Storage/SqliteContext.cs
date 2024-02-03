using Microsoft.EntityFrameworkCore;
using NewsMix.NewsSources;
using NewsMix.Storage.Entities;

namespace NewsMix.Storage;

public class SqliteContext(DbContextOptions<SqliteContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserAction> UserActions { get; set; } = null!;
    public DbSet<NotifiedPublication> NotifiedPublications { get; set; } = null!;
    public DbSet<NewsTopic> NewsTopics { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NewsTopic>().HasData
            (new List<NewsTopic>()
            {
                new()
                {
                    Id = 1,
                    InternalName = EaApex.ApexTopic,
                    NewsSource = "ea",
                    Enabled = true,
                    VisibleNameRU = "Apex Legends",
                    OrderInList = 1
                },
                
                new()
                {
                    Id = 2,
                    InternalName = Habr.rating25Topic,
                    NewsSource = "habr",
                    Enabled = true,
                    VisibleNameRU = "Рейтинг > 25",
                    OrderInList = 1
                },
                
                new()
                {
                    Id = 3,
                    InternalName = IcyVeins.Topic_wow,
                    NewsSource = "icyveins",
                    Enabled = true,
                    VisibleNameRU = "World of warcraft",
                    OrderInList = 1
                },
                new()
                {
                    Id = 4,
                    InternalName = IcyVeins.Topic_wow_classic,
                    NewsSource = "icyveins",
                    Enabled = true,
                    VisibleNameRU = "World of warcraft Classic",
                    OrderInList = 2,
                },
                new()
                {
                    Id = 5,
                    InternalName = IcyVeins.Topic_diablo,
                    NewsSource = "icyveins",
                    Enabled = true,
                    VisibleNameRU = "Diablo",
                    OrderInList = 3,
                },
                new()
                {
                    Id = 6,
                    InternalName = IcyVeins.Topic_warcraft,
                    NewsSource = "icyveins",
                    Enabled = true,
                    VisibleNameRU = "Warcraft",
                    OrderInList = 4,
                },
                new()
                {
                    Id = 7,
                    InternalName = IcyVeins.Topic_lost_arc,
                    NewsSource = "icyveins",
                    Enabled = true,
                    VisibleNameRU = "Lost Arc",
                    OrderInList = 5,
                },
                
                new()
                {
                    Id = 8,
                    InternalName = NoobClub.Topic_wow,
                    NewsSource = "noobclub",
                    Enabled = true,
                    VisibleNameRU = "World of Warcraft",
                    OrderInList = 1,
                },
                new()
                {
                    Id = 9,
                    InternalName = NoobClub.Topic_wow_classic,
                    NewsSource = "noobclub",
                    Enabled = true,
                    VisibleNameRU = "World of Warcraft Classic",
                    OrderInList = 2,
                },
                new()
                {
                    Id = 10,
                    InternalName = NoobClub.Topic_diablo,
                    NewsSource = "noobclub",
                    Enabled = true,
                    VisibleNameRU = "Diablo",
                    OrderInList = 3,
                },
                new()
                {
                    Id = 11,
                    InternalName = NoobClub.Topic_hs,
                    NewsSource = "noobclub",
                    Enabled = true,
                    VisibleNameRU = "Hearthstone",
                    OrderInList = 4,
                },
                new()
                {
                    Id = 12,
                    InternalName = NoobClub.Topic_overwatch,
                    NewsSource = "noobclub",
                    Enabled = true,
                    VisibleNameRU = "Overwatch",
                    OrderInList = 5,
                },
                new()
                {
                    Id = 13,
                    InternalName = NoobClub.Topic_w3,
                    NewsSource = "noobclub",
                    Enabled = true,
                    VisibleNameRU = "Warcraft 3",
                    OrderInList = 6,
                },
                new()
                {
                    Id = 14,
                    InternalName = NoobClub.Topic_blizzard,
                    NewsSource = "noobclub",
                    Enabled = true,
                    VisibleNameRU = "Blizzard",
                    OrderInList = 7,
                }
            });
    }
}