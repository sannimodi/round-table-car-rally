internal static class ConfigProvider
{
    public static string GetDataPath()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "CarRallyData");
    }

    public static string GetFontSourcePath()
    {
        return Path.Combine(GetDataPath(), "assets", "fonts");
    }

    public static string GetImageSourcePath()
    {
        return Path.Combine(GetDataPath(), "assets", "images");
    }

    public static string GetRallyConfigPath()
    {
        return Path.Combine(GetDataPath(), "config.csv");
    }

    public static string GetSpeedChartPath()
    {
        return Path.Combine(GetDataPath(), "speed_chart.csv");
    }

    public static string GetMarshalChartPath()
    {
        return Path.Combine(GetDataPath(), "marshal_chart.csv");
    }

    public static string GetMarshalDataPath()
    {
        return Path.Combine(GetResultPath(), "marshal_data.csv");
    }

    public static string GetMarshalsPath()
    {
        return Path.Combine(GetDataPath(), "marshals");
    }

    public static string GetResultPath()
    {
        var resultPath = Path.Combine(GetDataPath(), "result");

        // Ensure the result directory exists
        if (!Directory.Exists(resultPath))
        {
            Directory.CreateDirectory(resultPath);
        }

        return resultPath;
    }
}
