# ReportExtract

ReportExtract is a portable Windows desktop application for validating, filtering, and exporting structured Excel worksheets.

## Required worksheet columns

Every selected worksheet must have exactly these required headers (case-insensitive and trim-tolerant):

```text
CPN
Category
Manufacturer
MPN
```

Every eligible data row must contain a nonblank value for all four fields. A missing header, duplicate required header, or null, empty, or whitespace-only required value causes validation to fail and reports the affected Excel row numbers where applicable.

## Filtering and presets

`Manufacturer` is the only worksheet filter. Multiple Manufacturer selections use OR semantics; no selection exports from the complete eligible worksheet. Presets and last-used filter state persist Manufacturer selections only. Existing settings with removed fields are safely ignored.

## MAVL export

MAVL is calculated across the complete eligible worksheet. The generated `MAVL` column is immediately to the right of `CPN`.

The normal export algorithm starts with direct Manufacturer matches:

- A direct row with MAVL=`No` is included.
- A direct row belonging to a MAVL=`Yes` CPN expands to include every eligible row for that CPN, including competing manufacturers.
- Unrelated MAVL=`Yes` CPNs are not included.
- Source row order is preserved and each source row appears at most once.
- Direct matches are highlighted yellow; expansion-only rows are not highlighted.

The source workbook is never modified. Every exported workbook uses the worksheet name `Output File`, labels the logical part-number column `CPN`, and preserves useful values, formatting, dimensions, freeze panes, and AutoFilter availability where practical. When the source has AutoFilter enabled, the output gets a new full-result AutoFilter without carrying over source filter criteria.

## Building from source

Requires Windows and the .NET 8 SDK:

```powershell
dotnet build ReportExtract.sln
dotnet run --project ReportExtract.Phase1Tests\ReportExtract.Phase1Tests.csproj
```

The repository contains separate portable-distribution scripts; release packaging is not part of this source-change task.
