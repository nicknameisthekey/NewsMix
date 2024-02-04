using Microsoft.EntityFrameworkCore;
using NewsMix.NewsSources;
using NewsMix.Storage.Entities;

namespace NewsMix.Storage;

public class SqliteContext : DbContext
{
    public SqliteContext(DbContextOptions<SqliteContext> options) : base(options)
    {
        Database.Migrate();
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserAction> UserActions { get; set; } = null!;
    public DbSet<FoundPublication> FoundPublications { get; set; } = null!;
    public DbSet<NewsTopic> NewsTopics { get; set; } = null!;
    public DbSet<ActiveInlineQuery> ActiveInlineQueries { get; set; } = null!;
    public DbSet<NotificationTask> NotificationTasks { get; set; } = null!;

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
                    HashTag = "#ea #apex",
                    OrderInList = 1
                },
                
                new()
                {
                    Id = 2,
                    InternalName = Habr.rating25Topic,
                    NewsSource = "habr",
                    Enabled = true,
                    VisibleNameRU = "Статьи с рейтингом > 25",
                    HashTag = "#habr #rating25plus",
                    OrderInList = 1
                },
                
                new()
                {
                    Id = 3,
                    InternalName = IcyVeins.Topic_wow,
                    NewsSource = "icyveins",
                    Enabled = true,
                    VisibleNameRU = "World of Warcraft",
                    HashTag = "#icyveins #wow",
                    OrderInList = 1
                },
                new()
                {
                    Id = 4,
                    InternalName = IcyVeins.Topic_wow_classic,
                    NewsSource = "icyveins",
                    Enabled = true,
                    VisibleNameRU = "World of Warcraft Classic",
                    HashTag = "#icyveins #wowclassic",
                    OrderInList = 2,
                },
                new()
                {
                    Id = 5,
                    InternalName = IcyVeins.Topic_diablo,
                    NewsSource = "icyveins",
                    Enabled = true,
                    VisibleNameRU = "Diablo",
                    HashTag = "#icyveins #diablo",
                    OrderInList = 3,
                },
                new()
                {
                    Id = 6,
                    InternalName = IcyVeins.Topic_warcraft,
                    NewsSource = "icyveins",
                    Enabled = true,
                    VisibleNameRU = "Warcraft",
                    HashTag = "#icyveins #warcraft",
                    OrderInList = 4,
                },
                new()
                {
                    Id = 7,
                    InternalName = IcyVeins.Topic_lost_arc,
                    NewsSource = "icyveins",
                    Enabled = true,
                    VisibleNameRU = "Lost Arc",
                    HashTag = "#icyveins #lostarc",
                    OrderInList = 5,
                },
                
                new()
                {
                    Id = 8,
                    InternalName = NoobClub.Topic_wow,
                    NewsSource = "noobclub",
                    Enabled = true,
                    VisibleNameRU = "World of Warcraft",
                    HashTag = "#noobclub #wow",
                    OrderInList = 1,
                },
                new()
                {
                    Id = 9,
                    InternalName = NoobClub.Topic_wow_classic,
                    NewsSource = "noobclub",
                    Enabled = true,
                    VisibleNameRU = "World of Warcraft Classic",
                    HashTag = "#noobclub #wowclassic",
                    OrderInList = 2,
                },
                new()
                {
                    Id = 10,
                    InternalName = NoobClub.Topic_diablo,
                    NewsSource = "noobclub",
                    Enabled = true,
                    VisibleNameRU = "Diablo",
                    HashTag = "#noobclub #diablo",
                    OrderInList = 3,
                },
                new()
                {
                    Id = 11,
                    InternalName = NoobClub.Topic_hs,
                    NewsSource = "noobclub",
                    Enabled = true,
                    VisibleNameRU = "Hearthstone",
                    HashTag = "#noobclub #hearthstone",
                    OrderInList = 4,
                },
                new()
                {
                    Id = 12,
                    InternalName = NoobClub.Topic_overwatch,
                    NewsSource = "noobclub",
                    Enabled = true,
                    VisibleNameRU = "Overwatch",
                    HashTag = "#noobclub #overwatch",
                    OrderInList = 5,
                },
                new()
                {
                    Id = 13,
                    InternalName = NoobClub.Topic_w3,
                    NewsSource = "noobclub",
                    Enabled = true,
                    VisibleNameRU = "Warcraft",
                    HashTag = "#noobclub #warcraft",
                    OrderInList = 6,
                },
                new()
                {
                    Id = 14,
                    InternalName = NoobClub.Topic_blizzard,
                    NewsSource = "noobclub",
                    Enabled = true,
                    VisibleNameRU = "Blizzard",
                    HashTag = "#noobclub #blizzard",
                    OrderInList = 7,
                }
            });
    }
}