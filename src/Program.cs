// https://docs.microsoft.com/en-us/dotnet/core/diagnostics/metrics-instrumentation

using MonitoringExample;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

// Add services as metrics sources.
builder.Services
    .AddSingleton<ApiApplicationJob>()
    .AddHostedService<BatchApplicationJob>();

// Configure Exporter and Metrics Sources.
builder.Services.AddOpenTelemetryMetrics(b =>
{
    b.AddPrometheusExporter()
        .AddRuntimeMetrics()
        .AddAspNetCoreInstrumentation()
        .AddMeter(ApiApplicationJob.ApiApplicationJobMeter)
        .AddMeter(BatchApplicationJob.BatchApplicationJobMeter);
});

var app = builder.Build();

// Enable '/metrics' endpoint for Metrics pulling.
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapGet("/", (ApiApplicationJob job) => job.Execute());
app.Run();
