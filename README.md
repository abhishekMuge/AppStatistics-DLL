# CustomAppStatProvider

A lightweight **.NET application performance monitoring (APM) library** built for **development, testing, and learning purposes**.

This project is a **ground-up implementation of an APM-style agent**, inspired by tools like Atatus, Datadog, and Application Insights, but intentionally kept **transparent, hackable, and dependency-free**.

---

## 🎯 Project Goals

The primary goals of this project are:

* Understand **how APM agents work internally**
* Provide **application-level runtime statistics** with minimal overhead
* Enable **GC, CPU, and memory behavior analysis** in long-running .NET applications
* Serve as a **test harness / demo framework** for interviews, experiments, and profiling

This is **not a SaaS replacement** for commercial APMs — it is a **learning and experimentation tool**.

---

## ✨ Key Features

### Application Lifecycle

* Application start time (UTC)
* Uptime tracking
* Graceful shutdown handling

### CPU Metrics

* Process-level CPU utilization
* Core-normalized CPU percentage
* Stable sampling for long-running workloads

### Memory Metrics

* Current working set (MB)
* Peak working set tracking
* Managed heap visibility

### Garbage Collection Metrics

* Gen0 / Gen1 / Gen2 collection counts
* Clear visualization of allocation pressure
* LOH and promotion behavior (indirect)

### Custom Metrics

* User-defined gauges
* Thread-safe metric registry
* Extensible design for counters and histograms

### Runtime Characteristics

* Low allocation overhead
* No background thread explosion
* Cross-platform (Windows / Linux / macOS)
* Compatible with **.NET 6+ (tested up to net10.0)**

---

## 🧱 Architecture Overview

```
CustomAppStatProvider
├── Core
│   ├── ApmAgent.cs        // Public lifecycle API
│   └── ApmConfig.cs       // Configuration contract
│
├── Collectors
│   ├── CpuCollector.cs    // CPU utilization sampling
│   ├── MemoryCollector.cs // Working set + peak memory
│   └── GcCollector.cs     // GC counters
│
├── Metrics
│   └── MetricRegistry.cs  // Custom metric storage
│
└── Hosting
    └── ApmBackgroundWorker.cs // Periodic collection engine
```

Design principles:

* **Minimal public API surface**
* **Internal collectors** (hot-path code hidden)
* Clear separation between **collection** and **exposure**

---

## 🚀 Getting Started

### 1️⃣ Reference the Library

If using source:

```bash
dotnet add reference CustomAppStatProvider.csproj
```

If using DLL:

```bash
dotnet add reference CustomAppStatProvider.dll
```

---

### 2️⃣ Start the APM Agent

```csharp
using CustomAppStatProvider.Core;

var config = new ApmConfig
{
    AppName = "Demo-App",
    CollectionInterval = TimeSpan.FromSeconds(1),
    EnableConsoleLogging = true
};

ApmAgent.Instance.Start(config);
```

---

### 3️⃣ Shutdown Gracefully

```csharp
ApmAgent.Instance.Shutdown();
```

This ensures:

* Timers stop cleanly
* No background leaks
* Final metrics are flushed

---

## 🧪 Example Use Cases

### Long-Running Workload Analysis

* Observe GC behavior under sustained allocation pressure
* Compare Gen0 vs Gen1 vs Gen2 frequency
* Identify memory plateaus vs leaks

### Performance Demos

* Show CPU saturation patterns
* Demonstrate GC efficiency under load
* Explain .NET runtime behavior visually

### Interview / Learning Tool

* Explain GC generations with real data
* Show how CPU sampling works internally
* Demonstrate system design thinking

---

## 📊 Sample Output

```
CPU 50.2% | MEM 88MB | GC0:201 GC1:163 GC2:7
CPU 51.1% | MEM 89MB | GC0:204 GC1:166 GC2:7
CPU 48.9% | MEM 97MB | GC0:258 GC1:208 GC2:8
```

### Interpretation

* High GC0 → short-lived allocations
* Moderate GC1 → promoted objects
* Low GC2 → healthy memory behavior
* Stable memory → no leaks

---

## ⚠️ Limitations (By Design)

* No distributed tracing
* No automatic HTTP instrumentation
* No persistence backend
* No UI or dashboard

These are **intentional** to keep the system understandable and modifiable.

---

## 🔮 Planned Enhancements

* ASP.NET Core middleware
* PeriodicTimer-based scheduler
* Allocation rate tracking (MB/sec)
* GC pause time measurement
* Prometheus / OpenTelemetry exporter
* Lock-free metric buffers

---

## 🧠 Educational Value

This project demonstrates:

* Runtime instrumentation patterns
* GC internals in practice
* Low-overhead background monitoring
* API surface design for libraries
* Observability fundamentals

It is especially useful for engineers interested in:

* Systems programming
* Runtime internals
* Performance engineering
* Platform-level tooling

---

## 📄 License

MIT (or Internal Use)

---

## 🙌 Acknowledgements

Inspired by:

* Atatus
* Datadog APM
* Application Insights
* OpenTelemetry

Built intentionally from scratch to understand **what happens under the hood**.
