using System.Diagnostics;

namespace Pantry.Plan.Api.Configuration;

public static class DiagnosticsConfig
{
    public const string ServiceName = "Pantry.Plan.Api";
    public static ActivitySource ActivitySource = new(ServiceName);
}
