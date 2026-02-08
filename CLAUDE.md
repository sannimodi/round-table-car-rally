# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Round Table Car Rally Calculator - A .NET console application that calculates car rally results for Round Table events. The system processes speed reference charts, marshal points, and actual car arrival data to compute penalties and generate PDF result reports.

## Technology Stack

- .NET 10.0 (with AOT compilation enabled)
- C# with nullable reference types enabled
- xUnit for testing
- PDFsharp for PDF generation
- CSV for data input/output
- Spectre.Console for terminal UI

## Build and Run Commands

Build the solution:
```bash
dotnet build ResultCalculator/ResultCalculator.sln
```

Run the application:
```bash
dotnet run --project ResultCalculator/ResultCalculator/ResultCalculator.csproj
```

Run tests:
```bash
dotnet test ResultCalculator/ResultCalculatorTests/ResultCalculatorTests.csproj
```

## Data File Requirements

The application expects CSV files in `Desktop/CarRallyData/`:

### Input Files (Required)
- `config.csv` - Rally configuration (table name, date, time, participants, penalty values, rounding threshold)
  - **Required Columns**: All columns must be present or application will halt with error
    - Table Name, DATE, TIME, Participants
    - Early Penalty, Late Penalty, Missed Penalty, Extra Break Penalty
    - Rounding Threshold (0-59 seconds)
  - **Rounding Threshold**: Configurable value (0-59 seconds) that determines when time differences round up vs down
    - If remaining seconds >= threshold, rounds up to next minute
    - If remaining seconds < threshold, rounds down to current minute
    - Example with threshold=30: 5min 29sec → 5min, 5min 30sec → 6min, 5min 31sec → 6min
    - Default value: 30 (standard rounding)
- `speed_chart.csv` - Speed reference points (SR) with distance ranges and average speeds
- `marshal_chart.csv` - Marshal point locations and break durations
- `marshals/` folder - **REQUIRED**: Must contain at least one scan file for each marshal point
  - `marshals/{PointName}.*.csv` - Individual marshal point scan files (e.g., marshals/START.001.csv, marshals/MP1.001.csv)
  - **Validation**: Application will not proceed if any marshal point is missing scan files
- `assets/fonts/` - Custom fonts for PDF generation
- `assets/images/` - Images for PDF generation

### Output Files (Generated in `result/` subfolder)
- `result/marshal_data.csv` - Compiled marshal data from all scan files
- `result/{filename}.pdf` - Generated PDF reports

## Architecture

### Three-Stage Processing Pipeline

1. **Chart Compilation** (SegmentCompiler)
   - Merges speed reference chart with marshal points
   - Calculates expected time to reach each marshal point
   - Splits speed reference segments when marshal points fall within them
   - Generates CompiledSegment list with cumulative distances and times

2. **Data Reading** (Readers)
   - Reads CSV configuration files
   - Validates rally configuration constraints (penalty ranges, participant counts)
   - Parses marshal data with car numbers and scan times

3. **Result Calculation** (MarshalDataCompiler)
   - Processes actual arrival/departure times for each car
   - Calculates time penalties (early/late/missed marshal points)
   - Handles "proxy time" for missed marshal points (uses previous point + expected time)
   - Computes break duration penalties
   - Generates CarRallyResult objects with detailed breakdowns

### Key Models

- **RallyConfig**: Event metadata and penalty rules (EarlyPenalty: 1-5, LatePenalty: 1-3, MissedPenalty: 30-100)
- **SpeedReferencePoint**: Distance ranges with average speeds (in km/h)
- **MarshalPoint**: Checkpoint locations with break durations
- **CompiledSegment**: Calculated time/distance breakdown between checkpoints
- **CarRallyResult**: Per-car results with marshal point records and penalties

### Penalty Calculation Rules

- **Time Rounding**: All time differences use configurable threshold-based rounding (set via RoundingThresholdSeconds in config.csv)
  - Applied to: actual travel time calculations, break duration calculations
  - Ensures consistent rounding behavior across all penalty calculations
- **Early arrival**: penalty = EarlyPenalty x minutes_early (multiplier: 1-5)
- **Late arrival**: penalty = LatePenalty x minutes_late (multiplier: 1-3)
- **Missed marshal point**: fixed MissedPenalty (30-100 points)
- **Extra break time**: ExtraBreakPenalty x extra_minutes (multiplier: 0-2)
- **Proxy time**: When a marshal point is missed, departure time = previous_departure + expected_travel_time

### Break Time Handling (2025 Method)

For checkpoints with `BreakDuration > 0`, the system supports two scanning modes:

**Single-Scan Mode (Recommended)**:
- Only scan arrival/entry time at the break checkpoint
- System assumes departure time = arrival time + break duration
- NO break penalty is calculated at the break checkpoint
- Participants must manage their own break time
- Any extra/short break time appears as timing difference at next checkpoint

**Example CSV Data** (15-min break checkpoint):
```csv
# harekrishna.csv
Car Code,Time Captured
ART40/25/005,10:32:00
```
Result: Departure time calculated as 10:47:00 (10:32 + 15 min)

**Dual-Scan Mode (Legacy Support)**:
- Scan both entry and exit times (separated by pipe |)
- System calculates actual break duration
- Break penalty applied if exceeds allowed duration
- Backward compatible with 2024 format

**Example CSV Data** (15-min break checkpoint):
```csv
# harekrishna.csv
Car Code,Time Captured
ART40/25/005,10:32:00 | 10:50:00
```
Result: Departure time recorded as 10:50:00, break penalty calculated (18-15=3 extra minutes)

**Penalty Calculation Shift**:
- **Single-scan**: No break penalty at checkpoint, all timing appears at next checkpoint
- **Dual-scan**: Break penalty at checkpoint + timing penalty at next checkpoint
- Total penalty may differ slightly due to multiplier differences (LatePenalty vs ExtraBreakPenalty)

**Example Comparison** (participant takes 20 min break at 15-min checkpoint):
- Single-scan: 0 break penalty at CP, 5 min late at next CP (5 × LatePenalty)
- Dual-scan: 5 min break penalty at CP (5 × ExtraBreakPenalty), timing penalty at next CP

### Code Organization

- `Compilers/` - Chart compilation and result calculation logic
- `Readers/` - CSV file parsing (speed chart, marshal chart, config, marshal data)
- `Writers/` - PDF generation and CSV output
- `Models/` - Data models for rally components
- `ConfigProvider.cs` - Centralized file path configuration (points to Desktop/CarRallyData)

## Testing

The test project uses xUnit framework. Tests are located in `ResultCalculatorTests/`.

## PDF Output

The application generates PDF reports with:
- Rally configuration details
- Speed reference chart
- Marshal point breakdown
- Individual car results with penalties
- Custom fonts via CustomFontResolver

