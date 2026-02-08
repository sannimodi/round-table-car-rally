# Repository Guidelines

## Project Structure & Module Organization
This repo contains a .NET console app for rally result calculations and a small test project.
- `ResultCalculator/ResultCalculator/`: main application source (readers, compilers, writers, models).
- `ResultCalculator/ResultCalculatorTests/`: xUnit tests.
- `ResultCalculator/ResultCalculator.sln`: solution file.
- `data/`: sample CSV inputs and assets (fonts/images), plus historical event data folders.

## Build, Test, and Development Commands
Run these from the repository root:
- `dotnet build ResultCalculator/ResultCalculator.sln` — build the solution.
- `dotnet test ResultCalculator/ResultCalculator.sln` — run xUnit tests.
- `dotnet run --project ResultCalculator/ResultCalculator/ResultCalculator.csproj` — execute the console app.
- `dotnet publish ResultCalculator/ResultCalculator/ResultCalculator.csproj -c Release` — produce a release build (AOT is enabled in the project file).

## Runtime Data Setup
The app reads input data from the desktop path `~/Desktop/CarRallyData` (see `ConfigProvider`).
Copy or symlink sample inputs from `data/ART40 2024/` or `data/ART40 2023/` and ensure these files exist:
- `config.csv`
- `speed_chart.csv`
- `marshal_chart.csv`
- `marshal_data.csv` (optional; the app can generate this)
Also copy `data/assets/` into `~/Desktop/CarRallyData/assets` for PDF output.

## Coding Style & Naming Conventions
- C# with nullable reference types enabled; keep nullability annotations accurate.
- Use 4-space indentation and standard .NET formatting.
- Naming patterns: `PascalCase` for types/methods, `camelCase` for locals/parameters.
- Folder-based conventions: `Readers`, `Compilers`, `Writers`, `Models` map to class responsibilities; keep new classes aligned with these groupings.

## Testing Guidelines
- Framework: xUnit (`ResultCalculatorTests` project).
- Test file naming: `*Tests.cs`; test classes mirror the unit under test (e.g., `ConfigurationTests`).
- No explicit coverage threshold is configured; add tests for calculation logic or CSV parsing changes.

## Commit & Pull Request Guidelines
- Commit messages in history are short, sentence-style summaries (often with a period). Follow that pattern.
- PRs should include: a concise description, the sample data set used, and (if output changes) a PDF or console-output snippet.
