using System;
using System.Diagnostics.Tracing;

internal sealed class GCEventListener: EventListener
{
    private const string RuntimeProviderName = "Microsoft-Windows-DotNETRuntime";
    private const EventKeywords GcKeyword = (EventKeywords)0x1;

    private readonly GcPauseAggregator _aggregator;

    private long _gcStartTimestamp;

    public GCEventListener(GcPauseAggregator gcPauseAggregator)
    {
        _aggregator = gcPauseAggregator;
    }

    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        if (eventSource.Name == RuntimeProviderName)
        {
            EnableEvents(
                eventSource,
                EventLevel.Informational,
                GcKeyword
            );
        }
    }

    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        // Event IDs vary slightly by runtime version
        // So we key off EventName
        switch (eventData.EventName)
        {
            case "GCStart":
                _gcStartTimestamp = System.Diagnostics.Stopwatch.GetTimestamp();
                break;

            case "GCEnd":
                if (_gcStartTimestamp != 0)
                {
                    long end = System.Diagnostics.Stopwatch.GetTimestamp();
                    long deltaTicks = end - _gcStartTimestamp;

                    TimeSpan pause =
                        TimeSpan.FromSeconds(
                            (double)deltaTicks / System.Diagnostics.Stopwatch.Frequency);

                    _aggregator.RecordPause(pause);
                    _gcStartTimestamp = 0;
                }
                break;
        }
    }
}