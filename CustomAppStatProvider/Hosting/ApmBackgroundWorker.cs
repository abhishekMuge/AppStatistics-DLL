using AppStatProvider.Collectors;
using AppStatProvider.Metrics;
using AppStatProvider.Core;
using System.Timers;

namespace AppStatProvider.Hosting;

internal sealed class ApmBackgroundWorker
{
    // private readonly Timer _timer;
    private readonly System.Timers.Timer _timer;
    private readonly CpuCollector _cpu = new();
    private readonly MemoryCollector _memory = new();
    private readonly GcCollector _gc = new();
    private readonly ApmConfig _config;

    private readonly GcPauseAggregator _gcPauseAggregator = new();
    private readonly GCEventListener _gcEventListener;

    public ApmBackgroundWorker(ApmConfig config)
    {
        _config = config;

        
        _gcEventListener = new GCEventListener(_gcPauseAggregator);

        // _timer = new Timer(config.CollectionInterval.TotalMilliseconds);
        _timer = new System.Timers.Timer(config.CollectionInterval.TotalMilliseconds);
        _timer.Elapsed += (_, __) => Collect();
    }

    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();

    private void Collect()
    {
        var cpu = _cpu.Collect();
        var mem = _memory.Collect();

        var (totalPause, maxPause) = _gcPauseAggregator.SnapshotAndReset();

        if (_config.EnableConsoleLogging)
        {
            Console.WriteLine(
                $"CPU {cpu}% | MEM {mem / 1024 / 1024}MB | " +
                $"GC0:{_gc.Gen0} GC1:{_gc.Gen1} GC2:{_gc.Gen2} | " +
                $"GC Pause total:{totalPause.TotalMilliseconds:F1}ms " +
                $"max:{maxPause.TotalMilliseconds:F1}ms");
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
        _gcEventListener.Dispose();
    }
}
