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
- `config.csv` - Rally configuration (table name, date, time, participants, penalty values)
- `speed_chart.csv` - Speed reference points (SR) with distance ranges and average speeds
- `marshal_chart.csv` - Marshal point locations and break durations
- `marshal_data.csv` - Actual car arrival/departure times (auto-generated if missing)
- `assets/fonts/` - Custom fonts for PDF generation
- `assets/images/` - Images for PDF generation

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

- **Early arrival**: penalty = EarlyPenalty x minutes_early (multiplier: 1-5)
- **Late arrival**: penalty = LatePenalty x minutes_late (multiplier: 1-3)
- **Missed marshal point**: fixed MissedPenalty (30-100 points)
- **Extra break time**: ExtraBreakPenalty x extra_minutes (multiplier: 0-2)
- **Proxy time**: When a marshal point is missed, departure time = previous_departure + expected_travel_time

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

