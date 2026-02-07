using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== GC Pause Time Demonstration ===");
        Console.WriteLine("Press Ctrl+C to stop\n");

        // Enable full GC notifications
        GC.RegisterForFullGCNotification(10, 10);

        var gcMonitorThread = new Thread(GCMonitorLoop)
        {
            IsBackground = true
        };
        gcMonitorThread.Start();

        RunAllocationWorkload();
    }

    static void RunAllocationWorkload()
    {
        List<byte[]> heapPressure = new();
        Random rnd = new();

        while (true)
        {
            // Small object allocations (Gen0/Gen1 pressure)
            for (int i = 0; i < 10_000; i++)
            {
                var obj = new object();
            }

            // Large Object Heap allocations (forces expensive GC)
            byte[] lohObject = new byte[100_000 + rnd.Next(50_000)];
            heapPressure.Add(lohObject);

            // Prevent unbounded memory growth
            if (heapPressure.Count > 200)
            {
                heapPressure.RemoveRange(0, 100);
            }

            // Simulate real application pacing
            Thread.Sleep(20);
        }
    }

    static void GCMonitorLoop()
    {
        while (true)
        {
            var status = GC.WaitForFullGCApproach();

            if (status == GCNotificationStatus.Succeeded)
            {
                Console.WriteLine("\n[GC] Full GC approaching...");
                var sw = Stopwatch.StartNew();

                var completeStatus = GC.WaitForFullGCComplete();

                sw.Stop();

                if (completeStatus == GCNotificationStatus.Succeeded)
                {
                    PrintGCStats(sw.ElapsedMilliseconds);
                }
            }
        }
    }

    static void PrintGCStats(long pauseMs)
    {
        Console.WriteLine($"[GC] Full GC completed");
        Console.WriteLine($"     Pause Time   : {pauseMs} ms");
        Console.WriteLine($"     Gen0 Count   : {GC.CollectionCount(0)}");
        Console.WriteLine($"     Gen1 Count   : {GC.CollectionCount(1)}");
        Console.WriteLine($"     Gen2 Count   : {GC.CollectionCount(2)}");
        Console.WriteLine($"     Heap Size    : {GC.GetTotalMemory(false) / 1024 / 1024} MB");
        Console.WriteLine($"     Working Set  : {Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024} MB\n");
    }
}
