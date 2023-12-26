using System.Diagnostics.Metrics;

namespace Pantry.Api.Metrics;

public class PantryApiMetrics
{
    public const string MeterName = "Pantry.API";
    private readonly Counter<long> _pantryApiRequestsCounter;
    private readonly Histogram<double> _pantryApiRequestsDuration;

    public PantryApiMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);
        _pantryApiRequestsCounter = meter.CreateCounter<long>("pantry.api.requests.count", description: "Number of requests to the Pantry API");
        _pantryApiRequestsDuration = meter.CreateHistogram<double>("pantry.api.requests.duration", "ms", "Duration of requests to the Pantry API");
    }

    public void IncrementRequestsCounter()
    {
        _pantryApiRequestsCounter.Add(1);
    }

    public TrackerRequestsDuration TrackRequestsDuration()
    {
        return new TrackerRequestsDuration(_pantryApiRequestsDuration);
    }
}

public class TrackerRequestsDuration : IDisposable
{
    private readonly PantryApiMetrics _pantryApiMetrics;
    private readonly long _requestStartTime = TimeProvider.System.GetTimestamp();
    private readonly Histogram<double> _histogram;

    public TrackerRequestsDuration(Histogram<double> histogram)
    {
        _histogram = histogram;
    }

    public void Dispose()
    {
        var elapsed = TimeProvider.System.GetElapsedTime(_requestStartTime);
        _histogram.Record(elapsed.TotalMilliseconds);
    }
}