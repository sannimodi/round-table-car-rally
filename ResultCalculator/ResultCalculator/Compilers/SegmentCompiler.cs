using Microsoft.Extensions.Logging;

internal class SegmentCompiler(ILogger<SegmentCompiler> logger) : DataCompilerBase(logger)
{
    public List<CompiledSegment> CompileChart(List<SpeedReferencePoint> speedReferences, List<MarshalPoint> marshalPoints, int roundingThresholdSeconds)
    {
        _logger.LogInformation("Compiling the speed chart");

        var compiledSegments = new List<CompiledSegment>();
        double lastDistance = 0;
        double distanceDiff = 0;

        foreach (var speedReference in speedReferences)
        {
            var referenceMarshalPoints = marshalPoints.Where(mp => mp.Distance >= speedReference.FromKM && mp.Distance <= speedReference.ToKM);

            // Check for any MarshalPoints within this speed reference segment
            foreach (var marshalPoint in referenceMarshalPoints.ToList())
            {
                distanceDiff = Math.Round(marshalPoint.Distance - lastDistance, 2);

                if (distanceDiff < 0)
                {
                    _logger.LogError("Marshal chart is not in order");
                    return [];
                }

                var marshalSegment = new CompiledSegment
                {
                    PointName = marshalPoint.PointName,
                    IsMarshalPoint = true,
                    CumulativeDistance = marshalPoint.Distance,
                    DistanceFromLastPoint = distanceDiff,
                    ReferenceSpeed = speedReference.AverageSpeed,
                    TimeFromLastPoint = Math.Round((distanceDiff * 60) / speedReference.AverageSpeed, 2)
                };

                lastDistance = marshalPoint.Distance;
                compiledSegments.Add(marshalSegment);

                // Marshal time is the total time taken from last marshal point to the current marshal point
                // Skip the calculation if the marshal point is the first point
                if (marshalPoint.Distance == 0)
                {
                    marshalSegment.MarshalTime = 0;
                }
                else
                {
                    // Initialize the marshal time with the time required to reach the marshal point
                    double marshalTime = marshalSegment.TimeFromLastPoint;

                    for (var i = compiledSegments.Count - 2; i >= 0; i--)
                    {
                        if (compiledSegments[i].IsMarshalPoint)
                        {
                            break;
                        }
                        // Add the time taken to reach the previous point
                        marshalTime += compiledSegments[i].TimeFromLastPoint;
                    }

                    // Marshal time is the total time taken from last marshal point to the current marshal point
                    marshalSegment.MarshalTime = Math.Round(marshalTime, 2);

                    // Rounding off the marshal time to the nearest minute using threshold-based rounding
                    // This ensures consistency with actual time rounding (GetRoundedMinutesDifference)
                    marshalPoint.TimeToReach = DataExtensions.RoundToInt(marshalTime, roundingThresholdSeconds);
                }
            }

            distanceDiff = Math.Round(speedReference.ToKM - lastDistance, 2);

            if (distanceDiff < 0)
            {
                _logger.LogError("Speed chart is not in order");
                return [];
            }

            var segment = new CompiledSegment
            {
                PointName = speedReference.Reference,
                IsMarshalPoint = false,
                CumulativeDistance = speedReference.ToKM,
                DistanceFromLastPoint = distanceDiff,
                ReferenceSpeed = speedReference.AverageSpeed,
                TimeFromLastPoint = Math.Round((distanceDiff * 60) / speedReference.AverageSpeed, 2)
            };

            lastDistance = speedReference.ToKM;
            compiledSegments.Add(segment);
        }

        return compiledSegments;
    }
}