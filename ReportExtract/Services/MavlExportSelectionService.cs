using ReportExtract.Models;

namespace ReportExtract.Services;

public static class MavlExportSelectionService
{
    public static IReadOnlyList<ExportRowSelection> Create(
        WorksheetProcessingSession session,
        MavlResult mavl,
        IReadOnlyCollection<int> directMatchRowNumbers,
        string? showMavl)
    {
        var mode = FilterSelectionSnapshot.ShowMavlModes.Contains(showMavl, StringComparer.Ordinal)
            ? showMavl!
            : FilterSelectionSnapshot.FilteredOnlyMavlMode;
        var directRows = new HashSet<int>(directMatchRowNumbers);
        var qualifiedPartNumbers = session.EligibleRows
            .Where(row => directRows.Contains(row.ExcelRowNumber) && mavl.GetForExcelRow(row.ExcelRowNumber) == MavlClassification.Yes)
            .Select(row => row.PartNumberComparisonValue)
            .ToHashSet(StringComparer.Ordinal);

        var selected = new Dictionary<int, bool>();
        foreach (var row in session.EligibleRows)
        {
            var isDirect = directRows.Contains(row.ExcelRowNumber);
            var classification = mavl.GetForExcelRow(row.ExcelRowNumber);
            var include = mode switch
            {
                FilterSelectionSnapshot.FilteredOnlyMavlMode => isDirect,
                FilterSelectionSnapshot.NoOnlyMavlMode => isDirect && classification == MavlClassification.No,
                FilterSelectionSnapshot.YesOnlyMavlMode => classification == MavlClassification.Yes && qualifiedPartNumbers.Contains(row.PartNumberComparisonValue),
                FilterSelectionSnapshot.BothExpandedMavlMode =>
                    (isDirect && classification == MavlClassification.No) ||
                    (classification == MavlClassification.Yes && qualifiedPartNumbers.Contains(row.PartNumberComparisonValue)),
                _ => isDirect
            };

            if (include)
            {
                selected[row.ExcelRowNumber] = isDirect;
            }
        }

        var useHighlight = mode is FilterSelectionSnapshot.YesOnlyMavlMode or FilterSelectionSnapshot.BothExpandedMavlMode;
        return session.EligibleRows
            .Where(row => selected.ContainsKey(row.ExcelRowNumber))
            .Select(row => new ExportRowSelection(row.ExcelRowNumber, useHighlight && selected[row.ExcelRowNumber]))
            .ToList();
    }
}
