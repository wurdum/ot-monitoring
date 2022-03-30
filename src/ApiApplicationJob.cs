using System.Diagnostics.Metrics;

namespace MonitoringExample;

public class ApiApplicationJob : IDisposable
{
    public const string ApiApplicationJobMeter = nameof(ApiApplicationJobMeter);
    private static readonly Random Random = new();
    private readonly Meter _meter;
    private readonly Counter<int> _requests;
    private readonly Counter<int> _responses;
    private readonly Histogram<int> _latency;

    public ApiApplicationJob()
    {
        _meter = new Meter(ApiApplicationJobMeter);
        _requests = _meter.CreateCounter<int>("app-api-requests");
        _responses = _meter.CreateCounter<int>("app-api-responses");
        _latency = _meter.CreateHistogram<int>("app-api-latency");
    }

    public async Task<string> Execute()
    {
        // Record request.
        _requests.Add(1);

        // Do the job.
        var time = Random.Next(100);
        await Task.Delay(time);

        // Record time to do the job in a histogram.
        _latency.Record(time);

        // Record response.
        // Consider response successful if job is done faster than 50ms
        _responses.Add(1, new KeyValuePair<string, object?>("successful", time < 50));

        return "Hello World!";
    }

    public void Dispose()
    {
        _meter.Dispose();
    }
}
