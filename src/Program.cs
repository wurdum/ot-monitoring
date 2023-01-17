using MonitoringExample;
using OpenTelemetry;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

// Add services as metrics sources.
builder.Services
    .AddSingleton<ApiApplicationJob>()
    .AddHostedService<BatchApplicationJob>();

// Configure Exporter and Metrics Sources.
builder.Services.AddOpenTelemetry()
    .WithMetrics(b =>
    {
        b.AddPrometheusExporter()
            .AddRuntimeInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddMeter(ApiApplicationJob.ApiApplicationJobMeter)
            .AddMeter(BatchApplicationJob.BatchApplicationJobMeter);
    });

var app = builder.Build();

// Enable '/metrics' endpoint for Metrics pulling.
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapGet("/", (ApiApplicationJob job) => job.Execute());
app.Run();
