using System.Diagnostics;

namespace Pantry.Api.Configuration;

public static class DiagnosticsConfig
{
    public const string ServiceName = "Pantry.Api";
    public static ActivitySource ActivitySource = new(ServiceName);
}
