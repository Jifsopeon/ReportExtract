namespace ReportExtract.Models;

public sealed class FilterSelectionSnapshot
{
    public const string FilteredOnlyMavlMode = "Filtered Only";
    public const string YesOnlyMavlMode = "Yes ONLY";
    public const string NoOnlyMavlMode = "No ONLY";
    public const string BothExpandedMavlMode = "Both (Expanded)";

    public List<string> PartNumbers { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public List<string> Manufacturers { get; set; } = new();
    // Kept as a string so the persisted value is stable and readable.
    public string ShowMavl { get; set; } = FilteredOnlyMavlMode;

    public static IReadOnlyList<string> ShowMavlModes { get; } = new[]
    {
        FilteredOnlyMavlMode, YesOnlyMavlMode, NoOnlyMavlMode, BothExpandedMavlMode
    };

    public string NormalizedShowMavl => ShowMavlModes.Contains(ShowMavl, StringComparer.Ordinal)
        ? ShowMavl
        : FilteredOnlyMavlMode;

    public IReadOnlyList<string> ValuesFor(SessionFilterColumn column) => column switch
    {
        SessionFilterColumn.PartNumber => PartNumbers,
        SessionFilterColumn.Category => Categories,
        SessionFilterColumn.Manufacturer => Manufacturers,
        _ => Array.Empty<string>()
    };
}
