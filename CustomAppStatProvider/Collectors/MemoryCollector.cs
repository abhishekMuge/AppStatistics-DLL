using System.Diagnostics;

namespace AppStatProvider.Collectors;

internal sealed class MemoryCollector
{
    private readonly Process _process = Process.GetCurrentProcess();
    public long PeakBytes { get; private set; }

    public long Collect()
    {
        _process.Refresh();
        PeakBytes = Math.Max(PeakBytes, _process.WorkingSet64);
        return _process.WorkingSet64;
    }
}
