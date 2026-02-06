using AppStatProvider.Core;
using AppStatProvider.Metrics;
using System.Collections.Concurrent;

Console.WriteLine("Starting Long Running APM Demo...");

var config = new ApmConfig
{
    AppName = "LongRunning-GC-CPU-Demo",
    CollectionInterval = TimeSpan.FromSeconds(1),
    EnableConsoleLogging = true
};

ApmAgent.Instance.Start(config);

// Simulated cache (Gen2 survivor)
var cache = new ConcurrentDictionary<int, byte[]>();

// Cancellation support
using var cts = new CancellationTokenSource();

// 1️⃣ CPU Intensive Task
Task.Run(() => CpuWorker(cts.Token));

// 2️⃣ High Allocation Task (Gen0 / Gen1 pressure)
Task.Run(() => AllocationWorker(cts.Token));

// 3️⃣ LOH Allocations (Gen2 / Full GC)
Task.Run(() => LohWorker(cts.Token));

// 4️⃣ Cache Growth + Eviction
Task.Run(() => CacheWorker(cache, cts.Token));

// Run for long duration
Console.WriteLine("Running workload for 5 minutes...");
Thread.Sleep(TimeSpan.FromMinutes(5));

cts.Cancel();
ApmAgent.Instance.Shutdown();

Console.WriteLine("Demo finished.");



static void CpuWorker(CancellationToken token)
{
    while (!token.IsCancellationRequested)
    {
        double x = 0;
        for (int i = 1; i < 10_000_000; i++)
            x += Math.Sqrt(i);

        MetricRegistry.SetGauge("cpu.worker.last", x);
    }
}


static void AllocationWorker(CancellationToken token)
{
    var rnd = new Random();

    while (!token.IsCancellationRequested)
    {
        // Short-lived allocations
        var list = new List<byte[]>();

        for (int i = 0; i < 1000; i++)
            list.Add(new byte[rnd.Next(1_000, 10_000)]);

        // Let objects die quickly
        Thread.Sleep(100);
    }
}


static void LohWorker(CancellationToken token)
{
    while (!token.IsCancellationRequested)
    {
        // > 85KB goes to LOH
        var buffer = new byte[200_000];

        // Keep alive briefly
        Thread.Sleep(500);

        // Release → forces LOH cleanup only during full GC
    }
}


static void CacheWorker(
    ConcurrentDictionary<int, byte[]> cache,
    CancellationToken token)
{
    int key = 0;

    while (!token.IsCancellationRequested)
    {
        // Simulate cache growth
        cache[key++] = new byte[50_000]; // medium objects

        // Evict occasionally
        if (cache.Count > 200)
        {
            foreach (var k in cache.Keys.Take(50))
                cache.TryRemove(k, out _);
        }

        MetricRegistry.SetGauge("cache.entries", cache.Count);
        Thread.Sleep(200);
    }
}


