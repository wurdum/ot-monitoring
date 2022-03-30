# Monitoring with OpenTelemetry and System.Diagnostics.Metrics API

The current project demonstrates how the new [Metrics API of .NET](https://docs.microsoft.com/en-us/dotnet/core/diagnostics/metrics-instrumentation) can be used to produce metrics for Prometheus. It contains two sources of Metrics:

* `ApiApplicationJob` - represents handling HTTP requests
* `BatchApplicationJob` - represents background worker that does batch processing

To test the how Metrics are created and updated:

* Build and run project with `dotnet run --project src/MonitoringExample.csproj`
* Open multiple times endpoint `http://localhost:5000` to emulate HTTP requests
* Open `http://localhost:5000/metrics` endpoint to see produced Metrics

To view Metrics using [dotnet-counters](https://docs.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-counters):

* Install tool `dotnet tool install --global dotnet-counters`
* Run application
* Find application `pid` with `dotnet counters ps`
* Run tool `dotnet counters monitor --process-id {pid} --counters ApiApplicationJobMeter,BatchApplicationJobMeter`
