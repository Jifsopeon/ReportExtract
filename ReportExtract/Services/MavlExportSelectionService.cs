using ReportExtract.Models;

namespace ReportExtract.Services;

public static class MavlExportSelectionService
{
    public static IReadOnlyList<ExportRowSelection> Create(
        WorksheetProcessingSession session,
        MavlResult mavl,
        IReadOnlyCollection<int> directMatchRowNumbers)
    {
        var directRows = new HashSet<int>(directMatchRowNumbers);
        var qualifiedPartNumbers = session.EligibleRows
            .Where(row => directRows.Contains(row.ExcelRowNumber) && mavl.GetForExcelRow(row.ExcelRowNumber) == MavlClassification.Yes)
            .Select(row => row.PartNumberComparisonValue)
            .ToHashSet(StringComparer.Ordinal);

        var selected = new Dictionary<int, bool>();
        foreach (var row in session.EligibleRows)
        {
            var isDirect = directRows.Contains(row.ExcelRowNumber);
            var include = isDirect || qualifiedPartNumbers.Contains(row.PartNumberComparisonValue);

            if (include)
            {
                selected[row.ExcelRowNumber] = isDirect;
            }
        }

        return session.EligibleRows
            .Where(row => selected.ContainsKey(row.ExcelRowNumber))
            .Select(row => new ExportRowSelection(row.ExcelRowNumber, selected[row.ExcelRowNumber]))
            .ToList();
    }
}
