using AppStatProvider.Hosting;
using System.Diagnostics;

namespace AppStatProvider.Core;

public sealed class ApmAgent
{
    private static readonly Lazy<ApmAgent> _instance =
        new(() => new ApmAgent());

    public static ApmAgent Instance => _instance.Value;

    private ApmBackgroundWorker? _worker;
    private readonly Stopwatch _uptime = Stopwatch.StartNew();

    public DateTime StartTimeUtc { get; } = DateTime.UtcNow;

    private ApmAgent() { }

    public void Start(ApmConfig config)
    {
        if (_worker != null)
            return;

        _worker = new ApmBackgroundWorker(config);
        _worker.Start();
    }

    public void Shutdown()
    {
        _worker?.Stop();
        _worker = null;
        _uptime.Stop();
    }

    public TimeSpan Uptime => _uptime.Elapsed;
}
