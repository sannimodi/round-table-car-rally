internal class CarRallyResult
{
    public required int CarNumber { get; set; }

    public int RoundingThresholdSeconds { get; set; }

    public List<CarMarshalPointRecord> MarshalPointRecords { get; set; } = [];

    public int GetTotalPenalty => MarshalPointRecords.Sum(x => x.TimePenalty + x.BreakPenalty);

    internal class CarMarshalPointRecord
    {
        // Car Participant Code
        public required string PointName { get; set; }

        // Capture Data Points
        public TimeOnly[] ScannedData { get; set; } = [];

        // Actual time the car was scanned
        public TimeOnly? ArrivalTime { get; set; }

        // Actual departure time from the marshal point
        public TimeOnly? DepartureTime { get; set; }

        // If the car was missed
        public bool IsMissed { get; set; }

        // Break duration at the marshal point
        public int BreakDuration(int roundingThresholdSeconds)
        {
            if (ArrivalTime.HasValue && DepartureTime.HasValue)
            {
                return DataExtensions.GetRoundedMinutesDifference(
                    DepartureTime.Value,
                    ArrivalTime.Value,
                    roundingThresholdSeconds);
            }

            return 0;
        }

        // Best time to reach the marshal point
        public int BestTimeFromLastPoint { get; set; }

        // Time taken from the last point
        public int ActualTimeFromLastPoint { get; set; }

        // Difference between the actual and expected time
        public int TimeDifference { get; set; }

        // Penalty for the time difference
        public int TimePenalty { get; set; }

        // Break Penalty
        public int BreakPenalty { get; set; }
    }
}
