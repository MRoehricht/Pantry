using System.Diagnostics;

namespace Pantry.Module.Good.Configuration;

public static class DiagnosticsConfig
{
    public const string ServiceName = "Pantry.Module.Good";
    public static ActivitySource ActivitySource = new(ServiceName);
}
