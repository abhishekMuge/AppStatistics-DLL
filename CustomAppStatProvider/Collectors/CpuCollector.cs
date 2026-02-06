using System.Diagnostics;

namespace AppStatProvider.Collectors;

internal sealed class CpuCollector
{
    private readonly Process _process = Process.GetCurrentProcess();
    private TimeSpan _lastCpu;
    private DateTime _lastSample;

    public double Collect()
    {
        var now = DateTime.UtcNow;
        var cpu = _process.TotalProcessorTime;

        if (_lastSample == default)
        {
            _lastCpu = cpu;
            _lastSample = now;
            return 0;
        }

        var cpuDelta = (cpu - _lastCpu).TotalMilliseconds;
        var timeDelta = (now - _lastSample).TotalMilliseconds;

        _lastCpu = cpu;
        _lastSample = now;

        return Math.Round(
            cpuDelta / (timeDelta * Environment.ProcessorCount) * 100, 2);
    }
}
