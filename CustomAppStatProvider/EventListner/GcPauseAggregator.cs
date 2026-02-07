internal sealed class GcPauseAggregator
{
    public long TotalPauseTicks;
    public long MaxPauseTicks;

    public void RecordPause(TimeSpan pause)
    {
        long ticks = pause.Ticks;

        Interlocked.Add(ref TotalPauseTicks, ticks);

        long currentMax;
        while (ticks > (currentMax = Volatile.Read(ref MaxPauseTicks)))
        {
            if (Interlocked.CompareExchange(
                ref MaxPauseTicks,
                ticks,
                currentMax) == currentMax)
                break;
        }
    }

    public (TimeSpan total, TimeSpan max) SnapshotAndReset()
    {
        long total = Interlocked.Exchange(ref TotalPauseTicks, 0);
        long max = Interlocked.Exchange(ref MaxPauseTicks, 0);

        return (TimeSpan.FromTicks(total), TimeSpan.FromTicks(max));
    }
}