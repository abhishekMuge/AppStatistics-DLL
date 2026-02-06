namespace AppStatProvider.Core;

public sealed class ApmConfig
{
    public string AppName { get; init; } = "MiniApm-App";
    public TimeSpan CollectionInterval { get; init; } = TimeSpan.FromSeconds(1);
    public bool EnableConsoleLogging { get; init; } = false;
}