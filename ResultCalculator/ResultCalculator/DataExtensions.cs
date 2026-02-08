internal class DataExtensions
{
    public static string TimeOnlyString(TimeOnly? time)
    {
        return time.HasValue ? time.Value.ToString() : "";
    }
    
    public static string DoubleString(double? value)
    {
        return value.HasValue ? value.Value.ToString() : "";
    }

    /// <summary>
    /// Rounds a time value (in minutes) to the nearest integer using a threshold-based approach.
    /// This ensures consistency with actual time rounding used in GetRoundedMinutesDifference.
    /// </summary>
    /// <param name="timeInMinutes">The time value in minutes (e.g., 2.5, 3.7)</param>
    /// <param name="thresholdSeconds">Number of seconds to determine rounding (0-59).
    /// If fractional seconds >= threshold, rounds up; otherwise rounds down.</param>
    /// <returns>Rounded minutes as integer</returns>
    public static int RoundToInt(double timeInMinutes, int thresholdSeconds = 30)
    {
        int wholeMinutes = (int)Math.Floor(timeInMinutes);
        double fractionalMinutes = timeInMinutes - wholeMinutes;
        int fractionalSeconds = (int)Math.Round(fractionalMinutes * 60);

        return fractionalSeconds >= thresholdSeconds ? wholeMinutes + 1 : wholeMinutes;
    }

    /// <summary>
    /// Calculates the difference between two times and returns rounded minutes using a custom threshold.
    /// </summary>
    /// <param name="endTime">The later time</param>
    /// <param name="startTime">The earlier time</param>
    /// <param name="thresholdSeconds">Number of seconds to determine rounding (0-59).
    /// If remaining seconds >= threshold, rounds up; otherwise rounds down.</param>
    /// <returns>Rounded minutes as integer</returns>
    public static int GetRoundedMinutesDifference(TimeOnly endTime, TimeOnly startTime, int thresholdSeconds = 30)
    {
        TimeSpan timeSpan = endTime - startTime;
        int wholeMinutes = (int)Math.Floor(timeSpan.TotalMinutes);
        int remainingSeconds = timeSpan.Seconds;

        return remainingSeconds >= thresholdSeconds ? wholeMinutes + 1 : wholeMinutes;
    }
}