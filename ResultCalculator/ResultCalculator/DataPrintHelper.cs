using Humanizer;
using Spectre.Console;
using System.Globalization;

internal static class DataPrintHelper
{
    public static void PrintConfiguration(RallyConfig config)
    {
        // Create a table
        var table = new Table
        {
            // Add title
            Title = new TableTitle($"[underline red]{config.TableName}[/]")
        };

        // Add some columns
        table.AddColumn("Parameter");
        table.AddColumn("Value");

        // Add some rows
        table.AddRow("When", $"[green]{config.Date.Humanize(culture: new CultureInfo("en-US", false))}[/]");
        table.AddRow("Time", $"[green]{config.Time}[/]");
        table.AddRow("Participants", $"[green]{config.Participants}[/]");
        table.AddRow("Early Penalty", $"[green]{config.EarlyPenalty}[/]");
        table.AddRow("Late Penalty", $"[green]{config.LatePenalty}[/]");
        table.AddRow("Missed Penalty", $"[green]{config.MissedPenalty}[/]");
        table.AddRow("Break Penalty", $"[green]{config.ExtraBreakPenalty}[/]");
        table.AddRow("Rounding Threshold", $"[green]{config.RoundingThresholdSeconds}s[/]");

        // Print the table
        AnsiConsole.Write(table);
    }

    public static void PrintSpeedChart(List<SpeedReferencePoint> speedChart)
    {
        // Create a table
        var table = new Table
        {
            // Add title
            Title = new TableTitle($"[underline blue]Speed Chart[/]")
        };

        // Add column definitions
        table.AddColumn("Reference");
        table.AddColumn("From KM");
        table.AddColumn("To KM");
        table.AddColumn("Average Speed");

        foreach (var item in speedChart)
        {
            table.AddRow(item.Reference, item.FromKM.ToString(), item.ToKM.ToString(), item.AverageSpeed.ToString());
        }

        AnsiConsole.Write(table);
    }

    public static void PrintMarshalChart(List<MarshalPoint> marshalChart)
    {
        // Create a table
        var table = new Table
        {
            // Add title
            Title = new TableTitle($"[underline green]Marshal Chart {Environment.NewLine} {TimeSpan.FromMinutes(marshalChart.Sum(x => x.TimeToReach)).Humanize(2)}[/]")
        };

        // Add column definitions
        table.AddColumn("Point Name");
        table.AddColumn("Distance");
        table.AddColumn("TTR");
        table.AddColumns("BREAK");

        foreach (var item in marshalChart)
        {
            table.AddRow(item.PointName,
                item.Distance.ToString(),
                item.TimeToReach.ToString(),
                item.BreakDuration.ToString());
        }

        AnsiConsole.Write(table);
    }

    public static void PrintCompiledChart(List<CompiledSegment> compiledSegments)
    {
        // Create a table
        var table = new Table
        {
            // Add title
            Title = new TableTitle($"[underline purple]Compiled Chart[/]")
        };

        // Add some columns
        table.AddColumn("PointName");
        table.AddColumn("CD");
        table.AddColumn("RD");
        table.AddColumn("SPEED");
        table.AddColumn("TTR");
        table.AddColumn("MT");

        foreach (var item in compiledSegments)
        {
            table.AddRow(item.PointName ?? "",
                item.CumulativeDistance.ToString(),
                item.DistanceFromLastPoint.ToString(),
                item.ReferenceSpeed.ToString(),
                item.TimeFromLastPoint.ToString(),
                DataExtensions.DoubleString(item.MarshalTime));
        }

        AnsiConsole.Write(table);
    }

    internal static void PrintMarshalData(List<MarshalDataRecord> marshalRecords)
    {
        if (marshalRecords.Count == 0)
        {
            AnsiConsole.Markup("[red]No Marshal Data found[/]");
            return;
        }

        // Create a table
        var table = new Table
        {
            // Add title
            Title = new TableTitle($"[underline yellow]Marshal Data Chart[/]")
        };

        // Add some columns
        table.AddColumn("Car Code");

        foreach (var item in marshalRecords.First().MarshalScan)
        {
            table.AddColumn(item.Item1);
        }

        foreach (var item in marshalRecords)
        {
            List<string> values =
            [
                item.CarNumber.ToString()
            ];

            foreach (var dataPoint in item.MarshalScan)
            {
                values.Add(string.Join(" | ", dataPoint.Item2));
            }

            table.AddRow(values.ToArray());
        }

        AnsiConsole.Write(table);
    }

    internal static void PrintMarshalDataResults(List<CarRallyResult> results)
    {
        if (results.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No Marshal Data Results found[/]");
            return;
        }

        AnsiConsole.MarkupLine("[underline bold green]Round Table Rally Result[/]");

        foreach (var result in results)
        {
            var table = new Table
            {
                // Add title
                Title = new TableTitle($"[green]{result.CarNumber} Penalty: {result.GetTotalPenalty}[/]")
            };

            // Add some columns
            table.AddColumn("Point");
            table.AddColumn("AAT");
            table.AddColumn("ADT");
            table.AddColumn("Missed");
            table.AddColumn("Break");
            table.AddColumn("TT");
            table.AddColumn("BT");
            table.AddColumn("TD");
            table.AddColumn("Penalty");
            table.AddColumn("Data Points");

            foreach (var item in result.MarshalPointRecords)
            {
                table.AddRow(item.PointName,
                    DataExtensions.TimeOnlyString(item.ArrivalTime),
                    item.IsMissed ? $"[red]{DataExtensions.TimeOnlyString(item.DepartureTime)}[/]" : DataExtensions.TimeOnlyString(item.DepartureTime),
                    item.IsMissed ? "Y" : "N",
                    item.BreakDuration(result.RoundingThresholdSeconds).ToString(),
                    item.ActualTimeFromLastPoint.ToString(),
                    item.BestTimeFromLastPoint.ToString(),
                    item.TimeDifference.ToString(),
                    item.TimePenalty.ToString(),
                    string.Join(" | ", item.ScannedData));
            }

            AnsiConsole.Write(table);
        }

        // Print Result Table
        var resultTable = new Table
        {
            // Add title
            Title = new TableTitle($"[underline green]Result Table[/]")
        };

        // Add some columns
        resultTable.AddColumn("Car Code");
        resultTable.AddColumn("Time Started");
        resultTable.AddColumn("Time Finished");
        resultTable.AddColumn("Total Time");
        resultTable.AddColumn("Total Penalty");
        resultTable.AddColumn("Missed Marshals");

        foreach (var item in results)
        {
            var startTime = item.MarshalPointRecords.First().DepartureTime;
            var endTime = item.MarshalPointRecords.Last().ArrivalTime;

            var totalTime = (endTime.HasValue && startTime.HasValue) ? (endTime.Value - startTime.Value).TotalMinutes : 0;

            var totalMissedPoints = item.MarshalPointRecords.Count(x => x.IsMissed);

            resultTable.AddRow(item.CarNumber.ToString(),
                DataExtensions.TimeOnlyString(startTime),
                DataExtensions.TimeOnlyString(endTime),
                totalTime == 0 ? "" : TimeSpan.FromMinutes(totalTime).Humanize(2),
                item.GetTotalPenalty.ToString(),
                totalMissedPoints.ToString());
        }

        AnsiConsole.Write(resultTable);

        // Ordered Result Table
        var orderedResultTable = new Table
        {
            // Add title
            Title = new TableTitle($"[underline green]Result Table[/]")
        };

        // Add some columns
        orderedResultTable.AddColumn("Rank");
        orderedResultTable.AddColumn("Car Code");
        orderedResultTable.AddColumn("Time Started");
        orderedResultTable.AddColumn("Time Finished");
        orderedResultTable.AddColumn("Total Time");
        orderedResultTable.AddColumn("Total Penalty");
        orderedResultTable.AddColumn("Missed Marshals");
        int rank = 0;
        var lastPenalty = 0;
        int sameRankCount = 0;
        foreach (var item in results.OrderBy(x => x.GetTotalPenalty))
        {
            var startTime = item.MarshalPointRecords.First().DepartureTime;
            var endTime = item.MarshalPointRecords.Last().ArrivalTime;

            var totalTime = (endTime.HasValue && startTime.HasValue) ? (endTime.Value - startTime.Value).TotalMinutes : 0;

            var totalMissedPoints = item.MarshalPointRecords.Count(x => x.IsMissed);
            if (lastPenalty != item.GetTotalPenalty)
            {
                rank += sameRankCount;
                rank++;
                sameRankCount = 0;
            }
            orderedResultTable.AddRow(rank.ToString(), item.CarNumber.ToString(),
                DataExtensions.TimeOnlyString(startTime),
                DataExtensions.TimeOnlyString(endTime),
                totalTime == 0 ? "" : TimeSpan.FromMinutes(totalTime).Humanize(2),
                item.GetTotalPenalty.ToString(),
                totalMissedPoints.ToString());
            if (lastPenalty == item.GetTotalPenalty)
            {
                sameRankCount++;
            }
            lastPenalty = item.GetTotalPenalty;
        }

        AnsiConsole.Write(orderedResultTable);
    }
}
