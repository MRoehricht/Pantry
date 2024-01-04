using System.Diagnostics;

namespace Pantry.Api.Configuration;

public class DiagnosticsConfig
{
    public const string ServiceName = "Pantry.Api";
    public ActivitySource ActivitySource = new(ServiceName);
}
