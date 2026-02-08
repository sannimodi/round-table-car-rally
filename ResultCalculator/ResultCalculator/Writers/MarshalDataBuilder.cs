using Csv;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

internal sealed partial class MarshalDataBuilder(ILogger<MarshalDataBuilder> logger) : CsvReaderBase(logger)
{
    private const string pattern = @"\-(\d+)$";

    /// <summary>
    /// Scan CSV files named {pointName}.*.csv and validate the records
    /// CSV should have following columns
    ///     "Car Number" => car number
    ///     "Time" => time of the scan
    /// Records are validated for following
    ///     Time should be in HH:MM:SS format
    /// </summary>
    /// <param name="marshalPoints"></param>
    /// <param name="marshalRecords"></param>
    /// <returns></returns>
    public bool Build(RallyConfig config, List<MarshalPoint> marshalPoints)
    {
        // Validate marshals folder exists
        var marshalsPath = ConfigProvider.GetMarshalsPath();
        if (!Directory.Exists(marshalsPath))
        {
            _logger.FileNotFound("Marshals folder", marshalsPath);
            return false;
        }

        var headers = new List<string> { "Car Number" };

        headers.AddRange(marshalPoints.Select(x => x.PointName));

        List<(int, SortedList<TimeOnly, TimeOnly>[])> records = [];

        // Initialize a list of records for each car.
        // Total participants from config.
        // Total points are marshalPoints.
        for (int i = 0; i < config.Participants; i++)
        {
            var row = new SortedList<TimeOnly, TimeOnly>[marshalPoints.Count];

            for (int j = 0; j < marshalPoints.Count; j++)
            {
                row[j] = [];
            }

            records.Add((i + 1, row));
        }

        // Read all the files.
        for (int pIndex = 0; pIndex < marshalPoints.Count; pIndex++)
        {
            MarshalPoint? item = marshalPoints[pIndex];

            var files = Directory.GetFiles(ConfigProvider.GetMarshalsPath(), $"{item.PointName}.*.csv");

            if (files.Length == 0)
            {
                _logger.FileNotFound("MarshalData", $"marshals/{item.PointName}.*.csv");
                return false;
            }

            // Read each file.
            foreach (var file in files)
            {
                _logger.ReadingFile(file, item.PointName);

                var csv = File.ReadAllText(file);

                var lines = CsvReader.ReadFromText(csv, _csvOptions);

                if (!lines.Any())
                {
                    _logger.FileIsEmpty(file);
                    return false;
                }

                // Validate headers.
                var fileHeaders = lines.First().Headers;

                if (fileHeaders.First() != "Car Code")
                {
                    _logger.MissingCsvHeaderAtIndex(1, "Car Code", fileHeaders.First());
                    return false;
                }

                if (fileHeaders.Last() != "Time Captured")
                {
                    _logger.MissingCsvHeaderAtIndex(fileHeaders.Length, "Time Captured", fileHeaders.Last());
                    return false;
                }

                // Read each record.
                foreach (var line in lines)
                {
                    var carCode = line["Car Code"];

                    var match = CarCodeRegex().Match(carCode);

                    if (!match.Success)
                    {
                        _logger.InvalidDataFormat("Car Code", "Car Code should be a number between 1 and 999");
                        return false;
                    }

                    var carIndex = int.Parse(match.Groups[1].Value);

                    var row = records[carIndex - 1].Item2;

                    // Validating time against ISO 8601 format.
                    var timeStr = line["Time Captured"];

                    if (!DateTime.TryParse(timeStr, out DateTime dateTime))
                    {
                        _logger.InvalidDataFormat("Time Captured", "Time should be in HH:MM:SS format");
                        return false;
                    }

                    // check if same time captured for same car at same point.
                    var time = TimeOnly.FromDateTime(dateTime);

                    if (row[pIndex].Where(t => (time - t.Value).TotalSeconds < 15).Any())
                    {
                        _logger.SkippingDuplicateTime(carIndex, item.PointName);
                        continue;
                    }

                    row[pIndex].Add(time, time);
                }
            }
        }

        // row values
        List<string[]> rows = [];

        for (int i = 0; i < config.Participants; i++)
        {
            var rowValue = new List<string>();

            var record = records[i];

            rowValue.Add(record.Item1.ToString());

            for (int j = 0; j < marshalPoints.Count; j++)
            {
                var point = record.Item2[j];
                rowValue.Add(string.Join(" | ", point.Select(x => x.Value.ToString("HH:mm:ss"))));
            }

            rows.Add([.. rowValue]);
        }

        var csvWrite = CsvWriter.WriteToText([.. headers], rows, ',');

        File.WriteAllText(Path.Combine(ConfigProvider.GetResultPath(), "marshal_data.csv"), csvWrite);

        return true;
    }

    [GeneratedRegex(pattern)]
    private static partial Regex CarCodeRegex();
}