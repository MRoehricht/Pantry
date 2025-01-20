using System.Diagnostics;

namespace Pantry.Module.Plan.Configuration;

public static class DiagnosticsConfig
{
    public const string ServiceName = "Pantry.Module.Plan";
    public static ActivitySource ActivitySource = new(ServiceName);
}
