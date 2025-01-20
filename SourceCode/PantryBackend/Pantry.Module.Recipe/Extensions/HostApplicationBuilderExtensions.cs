using System.Reflection;

namespace Pantry.Module.Recipe.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddRecipeModule(this IHostApplicationBuilder builder,
        IList<Assembly> mediatrAssemblies)
    {
        mediatrAssemblies.Add(typeof(HostApplicationBuilderExtensions).Assembly);
        
        return builder;
    }
}