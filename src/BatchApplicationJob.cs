using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;

namespace MonitoringExample;

public class BatchApplicationJob : BackgroundService
{
    private readonly ILogger<BatchApplicationJob> _logger;
    public const string BatchApplicationJobMeter = nameof(BatchApplicationJobMeter);
    private static readonly Random Random = new();
    private readonly Meter _meter;
    private readonly Counter<int> _consumed;
    private readonly Histogram<int> _consumptionLatency;
    private readonly Counter<int> _published;
    private readonly Histogram<int> _publishingLatency;

    public BatchApplicationJob(ILogger<BatchApplicationJob> logger)
    {
        _logger = logger;
        _meter = new Meter(BatchApplicationJobMeter);
        _consumed = _meter.CreateCounter<int>("app-batch-records-consumed");
        _consumptionLatency = _meter.CreateHistogram<int>("app-batch-records-consumption-latency");
        _published = _meter.CreateCounter<int>("app-batch-records-published");
        _publishingLatency = _meter.CreateHistogram<int>("app-batch-records-publishing-latency");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Batch processing is started");

        // This method represents processing pipeline that consumes->maps->publishes records.
        await foreach (var record in ConsumeRecordsAsync(stoppingToken))
        {
            var newRecord = DoSomeJob(record);
            await PublishRecordAsync(newRecord);
        }

        _logger.LogInformation("Batch processing is stopped");
    }

    private async IAsyncEnumerable<int> ConsumeRecordsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var record = Random.Next(100);

            await Task.Delay(record, cancellationToken);

            _consumed.Add(1);
            _consumptionLatency.Record(record);

            yield return record;
        }
    }

    private async Task PublishRecordAsync(int record)
    {
        await Task.Delay(Random.Next(record));

        _published.Add(1);
        _publishingLatency.Record(record);
    }

    private static int DoSomeJob(int record)
    {
        return record + 1;
    }

    public override void Dispose()
    {
        _meter.Dispose();
        base.Dispose();
    }
}
