using Csv;
using Microsoft.Extensions.Logging;

internal class RallyConfigReader(ILogger<RallyConfigReader> logger) : CsvReaderBase(logger)
{
    public bool Read(out RallyConfig? config)
    {
        var configPath = ConfigProvider.GetRallyConfigPath();

        // Check if the file exists
        if (!File.Exists(configPath))
        {
            _logger.FileNotFound("Configuration", configPath);
            config = null;
            return false;
        }

        var csv = File.ReadAllText(configPath);

        var lines = CsvReader.ReadFromText(csv, _csvOptions);

        if(!lines.Any())
        {
            _logger.FileIsEmpty(configPath);
            config = null;
            return false;
        }

        if(lines.Count() != 1)
        {
            _logger.FileLineCountMismatch(1, lines.Count());
            config = null;
            return false;
        }

        // Get the first line
        var line = lines.First();

        // Validate all required columns exist
        var requiredColumns = new[]
        {
            "Table Name",
            "DATE",
            "TIME",
            "Participants",
            "Early Penalty",
            "Late Penalty",
            "Missed Penalty",
            "Extra Break Penalty",
            "Rounding Threshold"
        };

        var headers = line.Headers;
        foreach (var column in requiredColumns)
        {
            if (!headers.Contains(column))
            {
                _logger.MissingConfigColumn(column, configPath);
                config = null;
                return false;
            }
        }

        try
        {
            config = new RallyConfig
            {
                TableName = line["Table Name"],
                Date = DateOnly.ParseExact(line["DATE"], "ddMMyy"),
                Time = TimeOnly.ParseExact(line["TIME"], "HH:mm"),
                Participants = int.Parse(line["Participants"]),
                EarlyPenalty = int.Parse(line["Early Penalty"]),
                LatePenalty = int.Parse(line["Late Penalty"]),
                MissedPenalty = int.Parse(line["Missed Penalty"]),
                ExtraBreakPenalty = int.Parse(line["Extra Break Penalty"]),
                RoundingThresholdSeconds = int.Parse(line["Rounding Threshold"])
            };

            var results = config.Validate();
            if (results.Count > 0)
            {
                foreach (var validationResult in results)
                {
                    _logger.InvalidDataFormat(string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage);
                }
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.UnhandledException(ex.Message);
            config = null;
            return false;
        }        

        return true;
    }
}

