using System.Diagnostics;

namespace Pantry.Recipe.Api.Configuration;

public static class DiagnosticsConfig
{
    public const string ServiceName = "Pantry.Recipe.Api";
    public static ActivitySource ActivitySource = new(ServiceName);
}
