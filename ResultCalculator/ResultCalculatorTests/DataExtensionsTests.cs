namespace ResultCalculatorTests
{
    public class DataExtensionsTests
    {
        #region RoundToInt Tests

        [Theory]
        [InlineData(2.0, 30, 2)]  // Exact minute - no rounding needed
        [InlineData(2.5, 30, 3)]  // 2min 30sec - threshold = 30, should round up
        [InlineData(2.49, 30, 2)] // 2min 29.4sec - below threshold, should round down
        [InlineData(2.51, 30, 3)] // 2min 30.6sec - at/above threshold, should round up
        [InlineData(2.9, 30, 3)]  // 2min 54sec - above threshold, should round up
        [InlineData(3.0, 30, 3)]  // Exact minute
        public void RoundToInt_WithThreshold30_ShouldRoundCorrectly(double timeInMinutes, int threshold, int expected)
        {
            // Act
            var result = DataExtensions.RoundToInt(timeInMinutes, threshold);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(2.0, 40, 2)]   // Exact minute
        [InlineData(2.5, 40, 2)]   // 2min 30sec - below threshold 40, should round down
        [InlineData(2.67, 40, 3)]  // 2min 40sec - at threshold 40, should round up
        [InlineData(2.68, 40, 3)]  // 2min 41sec - above threshold 40, should round up
        [InlineData(2.65, 40, 2)]  // 2min 39sec - below threshold, should round down
        public void RoundToInt_WithThreshold40_ShouldRoundCorrectly(double timeInMinutes, int threshold, int expected)
        {
            // Act
            var result = DataExtensions.RoundToInt(timeInMinutes, threshold);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(2.0, 0, 3)]    // Exact minute - 0 seconds >= 0, rounds up
        [InlineData(2.01, 0, 3)]   // 2min 0.6sec - any seconds rounds up with threshold 0
        [InlineData(2.001, 0, 3)]  // 2min 0.06sec - rounds to 0 seconds, 0 >= 0, rounds up
        public void RoundToInt_WithThreshold0_ShouldRoundUpForAnySeconds(double timeInMinutes, int threshold, int expected)
        {
            // Act
            var result = DataExtensions.RoundToInt(timeInMinutes, threshold);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(2.0, 59, 2)]   // Exact minute - 0 seconds < 59, rounds down
        [InlineData(2.97, 59, 2)]  // 2min 58.2sec - rounds to 58 seconds, 58 < 59, rounds down
        [InlineData(2.98, 59, 3)]  // 2min 58.8sec - rounds to 59 seconds, 59 >= 59, rounds up
        [InlineData(2.99, 59, 3)]  // 2min 59.4sec - rounds to 59 seconds, 59 >= 59, rounds up
        public void RoundToInt_WithThreshold59_ShouldOnlyRoundUpAt59Seconds(double timeInMinutes, int threshold, int expected)
        {
            // Act
            var result = DataExtensions.RoundToInt(timeInMinutes, threshold);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void RoundToInt_WithDefaultThreshold_ShouldUse30Seconds()
        {
            // Arrange
            double timeInMinutes = 2.5; // 2min 30sec

            // Act
            var result = DataExtensions.RoundToInt(timeInMinutes);

            // Assert
            Assert.Equal(3, result); // Should round up with default threshold of 30
        }

        #endregion

        #region GetRoundedMinutesDifference Tests

        [Theory]
        [InlineData("10:00:00", "10:02:00", 30, 2)]   // Exact 2 minutes
        [InlineData("10:00:00", "10:02:29", 30, 2)]   // 2min 29sec - below threshold, round down
        [InlineData("10:00:00", "10:02:30", 30, 3)]   // 2min 30sec - at threshold, round up
        [InlineData("10:00:00", "10:02:31", 30, 3)]   // 2min 31sec - above threshold, round up
        [InlineData("10:00:00", "10:05:45", 30, 6)]   // 5min 45sec - above threshold, round up
        [InlineData("10:00:00", "10:05:15", 30, 5)]   // 5min 15sec - below threshold, round down
        public void GetRoundedMinutesDifference_WithThreshold30_ShouldRoundCorrectly(string startTime, string endTime, int threshold, int expected)
        {
            // Arrange
            var start = TimeOnly.Parse(startTime);
            var end = TimeOnly.Parse(endTime);

            // Act
            var result = DataExtensions.GetRoundedMinutesDifference(end, start, threshold);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("10:00:00", "10:02:00", 40, 2)]   // Exact 2 minutes
        [InlineData("10:00:00", "10:02:30", 40, 2)]   // 2min 30sec - below threshold 40, round down
        [InlineData("10:00:00", "10:02:39", 40, 2)]   // 2min 39sec - below threshold, round down
        [InlineData("10:00:00", "10:02:40", 40, 3)]   // 2min 40sec - at threshold, round up
        [InlineData("10:00:00", "10:02:41", 40, 3)]   // 2min 41sec - above threshold, round up
        public void GetRoundedMinutesDifference_WithThreshold40_ShouldRoundCorrectly(string startTime, string endTime, int threshold, int expected)
        {
            // Arrange
            var start = TimeOnly.Parse(startTime);
            var end = TimeOnly.Parse(endTime);

            // Act
            var result = DataExtensions.GetRoundedMinutesDifference(end, start, threshold);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("10:00:00", "10:02:00", 0, 3)]    // Exact 2 minutes - 0 seconds >= 0, rounds up
        [InlineData("10:00:00", "10:02:01", 0, 3)]    // 2min 1sec - any seconds rounds up with threshold 0
        [InlineData("10:00:00", "10:02:59", 0, 3)]    // 2min 59sec - rounds up
        public void GetRoundedMinutesDifference_WithThreshold0_ShouldRoundUpForAnySeconds(string startTime, string endTime, int threshold, int expected)
        {
            // Arrange
            var start = TimeOnly.Parse(startTime);
            var end = TimeOnly.Parse(endTime);

            // Act
            var result = DataExtensions.GetRoundedMinutesDifference(end, start, threshold);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("10:00:00", "10:02:00", 59, 2)]   // Exact 2 minutes
        [InlineData("10:00:00", "10:02:58", 59, 2)]   // 2min 58sec - below threshold 59, round down
        [InlineData("10:00:00", "10:02:59", 59, 3)]   // 2min 59sec - at threshold 59, round up
        public void GetRoundedMinutesDifference_WithThreshold59_ShouldOnlyRoundUpAt59Seconds(string startTime, string endTime, int threshold, int expected)
        {
            // Arrange
            var start = TimeOnly.Parse(startTime);
            var end = TimeOnly.Parse(endTime);

            // Act
            var result = DataExtensions.GetRoundedMinutesDifference(end, start, threshold);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetRoundedMinutesDifference_WithDefaultThreshold_ShouldUse30Seconds()
        {
            // Arrange
            var start = TimeOnly.Parse("10:00:00");
            var end = TimeOnly.Parse("10:02:30"); // 2min 30sec

            // Act
            var result = DataExtensions.GetRoundedMinutesDifference(end, start);

            // Assert
            Assert.Equal(3, result); // Should round up with default threshold of 30
        }

        #endregion

        #region Consistency Tests

        [Theory]
        [InlineData(2.0, "10:00:00", "10:02:00", 30)]   // Both exact 2 minutes
        [InlineData(2.5, "10:00:00", "10:02:30", 30)]   // Both 2.5 minutes (30 seconds)
        [InlineData(2.67, "10:00:00", "10:02:40", 40)]  // Both 2.67 minutes (40 seconds) with threshold 40
        [InlineData(3.99, "10:00:00", "10:03:59", 59)]  // Both 3.99 minutes (59 seconds) with threshold 59
        public void RoundingMethods_ShouldBeConsistent_WhenGivenEquivalentTimes(double timeInMinutes, string startTime, string endTime, int threshold)
        {
            // Arrange
            var start = TimeOnly.Parse(startTime);
            var end = TimeOnly.Parse(endTime);

            // Act
            var roundToIntResult = DataExtensions.RoundToInt(timeInMinutes, threshold);
            var getRoundedMinutesDifferenceResult = DataExtensions.GetRoundedMinutesDifference(end, start, threshold);

            // Assert
            Assert.Equal(roundToIntResult, getRoundedMinutesDifferenceResult);
        }

        [Theory]
        [InlineData(30)] // Standard rounding
        [InlineData(0)]  // Always round up
        [InlineData(40)] // Custom threshold
        [InlineData(59)] // Almost never round up
        public void RoundingMethods_ShouldProduceZeroPenalty_WhenExpectedEqualsActual(int threshold)
        {
            // Arrange - Create a scenario where expected time equals actual time
            double expectedTimeInMinutes = 2.5; // 2 minutes 30 seconds
            var actualStart = TimeOnly.Parse("10:00:00");
            var actualEnd = TimeOnly.Parse("10:02:30"); // Exactly 2 minutes 30 seconds

            // Act
            var expectedRounded = DataExtensions.RoundToInt(expectedTimeInMinutes, threshold);
            var actualRounded = DataExtensions.GetRoundedMinutesDifference(actualEnd, actualStart, threshold);

            // Calculate penalty (like the actual application does)
            var penalty = actualRounded - expectedRounded;

            // Assert
            Assert.Equal(0, penalty); // Should be zero penalty when times match
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void RoundToInt_WithZeroTime_ShouldReturnZero()
        {
            // Act
            var result = DataExtensions.RoundToInt(0.0, 30);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetRoundedMinutesDifference_WithSameTime_ShouldReturnZero()
        {
            // Arrange
            var time = TimeOnly.Parse("10:00:00");

            // Act
            var result = DataExtensions.GetRoundedMinutesDifference(time, time, 30);

            // Assert
            Assert.Equal(0, result);
        }

        [Theory]
        [InlineData(10.5, 30, 11)]   // 10 minutes 30 seconds
        [InlineData(15.75, 30, 16)]  // 15 minutes 45 seconds
        [InlineData(20.25, 30, 20)]  // 20 minutes 15 seconds
        public void RoundToInt_WithLargerTimes_ShouldRoundCorrectly(double timeInMinutes, int threshold, int expected)
        {
            // Act
            var result = DataExtensions.RoundToInt(timeInMinutes, threshold);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion
    }
}
