namespace Pantry.Module.Scanner.EanServices.DependencyInjection;
public static class EanServicesCollectionExtension
{
    public static IServiceCollection AddEanServices(this IServiceCollection services)
    {
        services.AddHttpClient<IEanProductFinderService, EanProductFinderService>().SetHandlerLifetime(TimeSpan.FromMinutes(5)); ;

        return services;
    }
}
