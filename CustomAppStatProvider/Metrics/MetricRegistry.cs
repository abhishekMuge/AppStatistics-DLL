using System.Collections.Concurrent;

namespace AppStatProvider.Metrics;

public static class MetricRegistry
{
    private static readonly ConcurrentDictionary<string, double> _gauges = new();

    public static void SetGauge(string name, double value)
        => _gauges[name] = value;

    internal static IReadOnlyDictionary<string, double> Snapshot()
        => _gauges;
}
