using Microsoft.Extensions.Logging;
using PdfSharp.Fonts;
using ResultCalculator.Writers;

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = factory.CreateLogger("Program");

logger.StartingApp("RallyCalculator");

var configReader = new RallyConfigReader(factory.CreateLogger<RallyConfigReader>());

if (!configReader.Read(out RallyConfig? config))
{
    return;
}

if (config == null)
{
    logger.RallyConfigReadFailed("Rally Configuration");
    return;
}

logger.RallyConfigReadSuccessful("Rally Configuration");

var speedChartReader = new SpeedChartReader(factory.CreateLogger<SpeedChartReader>());
if (!speedChartReader.Read(out List<SpeedReferencePoint> speedChart))
{
    return;
}

logger.RallyConfigReadSuccessful("Speed Chart");

var marshalReader = new MarshalChartReader(factory.CreateLogger<MarshalChartReader>());
if (!marshalReader.Read(out List<MarshalPoint> marshalChart))
{
    return;
}

logger.RallyConfigReadSuccessful("Marshal Chart");

// Compile the chart
var compiler = new SegmentCompiler(factory.CreateLogger<SegmentCompiler>());

var compiledChart = compiler.CompileChart(speedChart, marshalChart, config.RoundingThresholdSeconds);

DataPrintHelper.PrintConfiguration(config);
DataPrintHelper.PrintSpeedChart(speedChart);
DataPrintHelper.PrintMarshalChart(marshalChart);
DataPrintHelper.PrintCompiledChart(compiledChart);

Console.WriteLine("Do you want to continue? (Y/N)");
var choice = Console.ReadLine();

if (string.Compare(choice, "Y", true) != 0)
{
    return;
}

// Reader Marshal Data if available
if (!File.Exists(ConfigProvider.GetMarshalDataPath()))
{
    logger.FileNotFound("Marshal Data", ConfigProvider.GetMarshalDataPath());

    var marshalDataBuilder = new MarshalDataBuilder(factory.CreateLogger<MarshalDataBuilder>());
    if (!marshalDataBuilder.Build(config, marshalChart))
    {
        return;
    }
}

var marshalDataReader = new MarshalDataReader(factory.CreateLogger<MarshalDataReader>());
if (!marshalDataReader.Read(marshalChart, out List<MarshalDataRecord> marshalRecords))
{
    return;
}

DataPrintHelper.PrintMarshalData(marshalRecords);

var marshalDataCompiler = new MarshalDataCompiler(factory.CreateLogger<MarshalDataCompiler>());
var results = marshalDataCompiler.CompileMarshalData(config, marshalChart, marshalRecords);

DataPrintHelper.PrintMarshalDataResults(results);

GlobalFontSettings.FontResolver = new CustomFontResolver();

ResultWriter.CreateRallyResultPdf(results);

logger.ShuttingDown();

Console.ReadLine();
