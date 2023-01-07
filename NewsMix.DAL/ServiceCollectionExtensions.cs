using Microsoft.Extensions.DependencyInjection;
using NewsMix.DAL.Repositories.Abstraction;

namespace NewsMix.DAL;
public static class SerivceCollectionExtensions
{
    public static void AddFileRepository(this IServiceCollection services)
    {
        services.AddSingleton<UserRepository, FileRepository>();
        services.AddSingleton<PublicationRepository, FileRepository>();
    }
}