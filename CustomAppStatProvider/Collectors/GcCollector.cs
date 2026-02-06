namespace AppStatProvider.Collectors;

internal sealed class GcCollector
{
    public int Gen0 => GC.CollectionCount(0);
    public int Gen1 => GC.CollectionCount(1);
    public int Gen2 => GC.CollectionCount(2);
}