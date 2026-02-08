using Microsoft.Extensions.Logging;

internal static partial class LoggingExtensions
{
    // Starting: {AppName}
    [LoggerMessage(1001, LogLevel.Information, "Starting: {AppName}")]
    public static partial void StartingApp(this ILogger logger, string? appName);

    // Application is shutting down
    [LoggerMessage(1002, LogLevel.Information, "Application is shutting down")]
    public static partial void ShuttingDown(this ILogger logger);

    // {FileType} file not found at: {FilePath}
    [LoggerMessage(LogLevel.Error, "{FileType} file not found at: {FilePath}")]
    public static partial void FileNotFound(this ILogger logger, string fileType, string filePath);

    // Reading {FileName} file for {MarshalPoint} point.
    [LoggerMessage(LogLevel.Information, "Reading {FileName} file for {MarshalPoint} point.")]
    public static partial void ReadingFile(this ILogger logger, string fileName, string marshalPoint);

    // File is empty
    [LoggerMessage(LogLevel.Error, "File is empty: {FilePath}")]
    public static partial void FileIsEmpty(this ILogger logger, string filePath);

    // File is expecting {Number} of lines. Found {Number} lines.
    [LoggerMessage(LogLevel.Error, "File is expecting {Expected} of lines. Found {Actual} lines.")]
    public static partial void FileLineCountMismatch(this ILogger logger, int expected, int actual);

    // Rally Configuration File read successfully
    [LoggerMessage(LogLevel.Information, "Rally Configuration File {ConfigurationName} read successfully")]
    public static partial void RallyConfigReadSuccessful(this ILogger logger, string configurationName);

    // Rally Configuration File read failed
    [LoggerMessage(LogLevel.Error, "Rally Configuration File {ConfigurationName} read failed")]
    public static partial void RallyConfigReadFailed(this ILogger logger, string configurationName);

    // Log invalid data format exception
    [LoggerMessage(LogLevel.Error, "Invalid data format: {MemberNames} : {Message}")]
    public static partial void InvalidDataFormat(this ILogger logger, string memberNames, string? message);

    // Log missing CSV header
    [LoggerMessage(LogLevel.Error, "Missing CSV header: {ExpectedHeader}")]
    public static partial void MissingCsvHeader(this ILogger logger, string expectedHeader);

    // Log missing required column in config file
    [LoggerMessage(LogLevel.Error, "Missing required column '{ColumnName}' in configuration file: {FilePath}")]
    public static partial void MissingConfigColumn(this ILogger logger, string columnName, string filePath);

    // Validating csv headers
    [LoggerMessage(LogLevel.Information, "Validating csv headers.")]
    public static partial void ValidatingCsvHeaders(this ILogger logger);

    // Missing time captured for car number at marshal point
    [LoggerMessage(LogLevel.Warning, "Missing time captured for car number: {CarNumber} at marshal point: {MarshalPoint}")]
    public static partial void MissingTimeCaptured(this ILogger logger, int carNumber, string marshalPoint);

    // Skipping duplicate time for car number at marshal point
    [LoggerMessage(LogLevel.Warning, "Skipping duplicate time for car number: {CarNumber} at marshal point: {MarshalPoint}")]
    public static partial void SkippingDuplicateTime(this ILogger logger, int carNumber, string marshalPoint);

    // Log marshal data read line information
    [LoggerMessage(LogLevel.Information, "Reading data line: {Line}")]
    public static partial void ReadingDataLine(this ILogger logger, string line);

    // Log missing CSV header at index
    [LoggerMessage(LogLevel.Error, "Missing CSV header at index: {Index}. Expected: {ExpectedHeader}, Actual: {ActualHeader}")]
    public static partial void MissingCsvHeaderAtIndex(this ILogger logger, int index, string expectedHeader, string actualHeader);

    // Log unhandled exception
    [LoggerMessage(LogLevel.Error, "Unhandled exception: {Message}")]
    public static partial void UnhandledException(this ILogger logger, string message);
}