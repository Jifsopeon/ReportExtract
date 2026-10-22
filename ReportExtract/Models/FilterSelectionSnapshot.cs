namespace ReportExtract.Models;

public sealed class FilterSelectionSnapshot
{
    public List<string> Manufacturers { get; set; } = new();

    public IReadOnlyList<string> ValuesFor(SessionFilterColumn column) => column switch
    {
        SessionFilterColumn.Manufacturer => Manufacturers,
        _ => Array.Empty<string>()
    };
}
