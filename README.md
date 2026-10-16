# ReportExtract

ReportExtract is a portable Windows desktop application for filtering, validating, processing, and exporting structured Excel worksheets.

It is designed to reduce repetitive worksheet-processing work by allowing users to select a worksheet, apply reusable filters, calculate MAVL status, and export only the relevant records into a new Excel workbook.

## Features

- Import `.xlsx` workbooks
- Select a worksheet for processing
- Optionally include hidden rows and columns
- Validate required worksheet structure before processing
- Filter by `CPN`, `Category`, and `Manufacturer`
- Control MAVL export behavior using:
  - `Filtered Only`
  - `Yes ONLY`
  - `No ONLY`
  - `Both (Expanded)`
- Save and reuse filter presets
- Search available filter values
- Preserve unavailable preset selections and warn before export
- Automatically calculate MAVL status from the complete eligible worksheet
- Expand MAVL=`Yes` results to include competing manufacturer rows when requested
- Highlight direct filter matches in expanded MAVL exports
- Export only the selected result rows into a new workbook
- Preserve useful worksheet characteristics including source values, value types, formatting, column widths, row heights, header formatting, and freeze panes
- Show export progress and allow cancellation
- Store settings, temporary data, and logs inside the portable application folder
- Run as a self-contained Windows x64 application without requiring a separate .NET runtime

## Required Worksheet Columns

The selected worksheet must contain these logical columns:

```text
CPN
Category
Manufacturer
MPN1
```

### CPN compatibility

ReportExtract uses `CPN` as the current user-facing name.

For input compatibility, the CPN field may be provided by either worksheet header:

```text
CPN
P+F part number
```

These represent the same logical column. If both appear in the same worksheet, validation fails because the logical CPN field is duplicated/ambiguous.

Exported workbooks always use:

```text
CPN
```

even when the imported workbook used the legacy `P+F part number` header.

Header matching ignores leading/trailing whitespace and letter case, but otherwise requires the expected header names.

### Empty Cells

Blank cells are permitted in general worksheet data.

The following fields are required for every eligible data row:

```text
CPN
Manufacturer
```

A blank, empty, or whitespace-only value in either field causes worksheet validation to fail.

`Category` may be blank. Blank categories appear as `(Blank)` in the filter interface.

`MPN1` is required as a worksheet column, but its individual cell values may be blank.

Other worksheet columns may also contain empty cells.

Fatal worksheet-validation errors are shown in a modal error dialog and retained in the application status area.

## MAVL

ReportExtract generates a `MAVL` column immediately to the right of `CPN`.

MAVL is calculated from the complete eligible worksheet after validation/loading and before user filters are applied.

For each CPN:

```text
More than one distinct Manufacturer -> Yes
One distinct Manufacturer           -> No
```

Repeated rows from the same manufacturer do not cause MAVL to become `Yes`.

`Category` does not affect MAVL calculation.

Because MAVL is determined before filtering, changing filters does not recalculate MAVL.

## Filtering

The normal worksheet filters are:

```text
CPN
Category
Manufacturer
```

Filtering rules:

- Multiple selections within the same field use **OR**
- Different filter fields use **AND**
- No selections for a field means that field is not filtered

Example:

```text
Category:
Electrical OR Mechanical

Manufacturer:
Manufacturer A OR Manufacturer B
```

is evaluated as:

```text
(Electrical OR Mechanical)
AND
(Manufacturer A OR Manufacturer B)
```

Leading and trailing whitespace is ignored for analytical comparison. Internal whitespace and case are otherwise preserved.

Rows produced by these normal filters are the **direct matches**.

## Show MAVL

`Show MAVL` controls how the direct filter result becomes the final export.

The default option is:

```text
Filtered Only
```

### Filtered Only

Exports only the rows that directly match the CPN, Category, and Manufacturer filters.

No MAVL-based expansion is performed.

### Yes ONLY

Starts from the direct filter result and keeps only CPNs whose calculated MAVL value is `Yes`.

For every qualifying CPN, ReportExtract then includes all eligible worksheet rows belonging to that CPN so that competing manufacturer records are visible.

These added competing rows do not need to satisfy the original Category or Manufacturer filters.

Example:

```text
Source rows:

CPN    Category   Manufacturer
A100   IC         Manufacturer A
A100   Sensor     Manufacturer B
B200   IC         Manufacturer A
```

With:

```text
Category = IC
Manufacturer = Manufacturer A
Show MAVL = Yes ONLY
```

the direct result contains:

```text
A100 / IC / Manufacturer A
B200 / IC / Manufacturer A
```

If `A100` has MAVL=`Yes` and `B200` has MAVL=`No`, the final export contains:

```text
A100 / IC     / Manufacturer A
A100 / Sensor / Manufacturer B
```

The second row is added to show the competing manufacturer that contributes to the MAVL=`Yes` classification.

### No ONLY

Exports only direct matches whose calculated MAVL value is `No`.

No competing-row expansion is performed.

### Both (Expanded)

Combines the expanded MAVL=`Yes` behavior with the direct MAVL=`No` result.

Conceptually:

```text
expanded Yes ONLY result
+
direct No ONLY result
```

MAVL=`Yes` CPNs are expanded to include their competing rows.

MAVL=`No` rows remain limited to the original direct matches.

Unrelated MAVL=`Yes` CPNs that did not have a direct match are not added.

## Expanded-Result Highlighting

Special row highlighting is used only for:

```text
Yes ONLY
Both (Expanded)
```

In these modes:

- rows that directly matched the user's original filters are highlighted
- rows added only because of MAVL expansion are not highlighted

This distinguishes the user's original filter result from competing-manufacturer rows added for MAVL context.

Each source row is exported at most once. A direct-match row is not duplicated when the same CPN is expanded.

For `Filtered Only` and `No ONLY`, no special direct-match highlighting is applied.

## Filter Presets

Filter selections can be saved as presets and reused later.

Presets store selections for:

```text
CPN
Category
Manufacturer
Show MAVL
```

ReportExtract also persists the last-used filter selections and basic application preferences.

If older settings or presets do not contain a `Show MAVL` value, ReportExtract defaults to:

```text
Filtered Only
```

If a preset contains a filter value that is not available in the currently loaded worksheet, ReportExtract preserves the selection and warns the user rather than silently discarding it.

## Export

ReportExtract creates a new `.xlsx` workbook containing the rows selected by the active filters and `Show MAVL` mode.

The source workbook is never modified.

The exported worksheet:

- always labels the logical part-number column as `CPN`
- inserts the generated `MAVL` column immediately to the right of `CPN`
- preserves source values and underlying Excel value types where practical
- preserves useful layout information such as formatting, column widths, row heights, and freeze panes
- preserves original worksheet row order for the selected result
- suppresses duplicate source rows

Complex workbook structures such as conditional-formatting rules, named ranges, data validation, tables, and other advanced workbook metadata are not the primary focus of the export.

If no rows match the active selection, ReportExtract displays a notification and does not create an output workbook.

## Download and Run

ReportExtract is distributed as a self-contained Windows x64 package.

No installation of Visual Studio, the .NET SDK, Python, or other development tools is required to run the published application.

1. Download `ReportExtract-win-x64.zip` from the GitHub Releases page.
2. Extract the ZIP completely.
3. Open the extracted `ReportExtract-win-x64` folder.
4. Run:

```text
ReportExtract.exe
```

Do not run the application directly from inside the ZIP archive.

The top-level `ReportExtract.exe` is a lightweight launcher that starts the actual application from the `Backend` folder. Keep the complete extracted folder together.

## Portable Folder Layout

```text
ReportExtract-win-x64/
├── ReportExtract.exe
├── Backend/
│   ├── ReportExtract.exe
│   ├── .NET runtime files
│   ├── DuckDB dependencies
│   └── other application dependencies
└── Data/
    ├── settings.json
    ├── Temp/
    └── Logs/
```

The `Data` directory contains writable application state:

- `settings.json` — presets and application preferences
- `Temp/` — temporary DuckDB session data
- `Logs/` — bounded performance/runtime logs

`settings.json` is created when settings are first saved.

ReportExtract does not require LocalAppData for active settings storage.

Extract the application to a location where the current user has write permission, such as `Documents` or `Desktop`.

Avoid protected locations such as `Program Files`.

Deleting the extracted ReportExtract folder removes the portable settings, temporary data, logs, and runtime files together.

## Technology

ReportExtract is built with:

- C#
- .NET 8
- WPF
- ClosedXML
- DuckDB

## Building from Source

### Requirements

For normal development:

- Windows
- .NET 8 SDK
- Visual Studio or another suitable .NET development environment

Build the solution with:

```powershell
dotnet build ReportExtract.sln
```

## Creating the Portable Windows Package

The repository includes:

```text
publish-win-x64.ps1
Launcher/ReportExtractLauncher.c
```

These files work together to create the distributable package.

`publish-win-x64.ps1`:

- restores the `win-x64` runtime packages when required
- publishes ReportExtract as a self-contained .NET 8 Windows application
- places the application runtime in `Backend`
- compiles `ReportExtractLauncher.c` into the top-level `ReportExtract.exe`
- creates the portable `Data` directories
- validates required DuckDB/runtime files
- creates `dist/ReportExtract-win-x64.zip`

Building the portable launcher requires Visual Studio Build Tools with the **Desktop development with C++** workload.

From the repository root:

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\publish-win-x64.ps1
```

Generated output:

```text
dist/
├── ReportExtract-win-x64/
└── ReportExtract-win-x64.zip
```

The `dist` directory is generated output and should not be committed to source control.

## Repository Structure

```text
ReportExtract/
├── Launcher/
│   └── ReportExtractLauncher.c
├── ReportExtract/
│   ├── Models/
│   ├── Services/
│   ├── Utilities/
│   ├── ViewModels/
│   ├── Views/
│   └── ReportExtract.csproj
├── README.md
├── ReportExtract.sln
└── publish-win-x64.ps1
```

## Supported Platform

Current release target:

```text
Windows x64
```

The published release is self-contained and does not require a separately installed .NET runtime.

## Usage and Support

ReportExtract is provided as a general-purpose utility for processing structured Excel worksheets.

Support, maintenance, future updates, and compatibility with future operating-system or dependency changes are not guaranteed.

Always retain appropriate backups of important source workbooks and exported data.
