using System.Diagnostics;

namespace Pantry.Module.Recipe.Configuration;

public static class DiagnosticsConfig
{
    public const string ServiceName = "Pantry.Module.Recipe";
    public static ActivitySource ActivitySource = new(ServiceName);
}
