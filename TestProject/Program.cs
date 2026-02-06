using AppStatProvider.Core;
using AppStatProvider.Metrics;

Console.WriteLine("Starting MiniAPM test...");

var config = new ApmConfig
{
    AppName = "Console-Test",
    CollectionInterval = TimeSpan.FromSeconds(1),
    EnableConsoleLogging = true
};

ApmAgent.Instance.Start(config);

// Simulate CPU load
for (int i = 0; i < 5; i++)
{
    BusyWork();
    MetricRegistry.SetGauge("loop.iteration", i);
    Thread.Sleep(500);
}

ApmAgent.Instance.Shutdown();
Console.WriteLine("Test finished.");

static void BusyWork()
{
    var sum = 0.0;
    for (int i = 0; i < 5_000_000; i++)
        sum += Math.Sqrt(i);
}

