using System.ComponentModel.DataAnnotations;

internal class RallyConfig
{
    public required string TableName { get; set; }

    public required DateOnly Date { get; set; }

    public required TimeOnly Time { get; set; }

    public required int Participants { get; set; }

    public required int EarlyPenalty { get; set; }

    public required int LatePenalty { get; set; }

    public required int MissedPenalty { get; set; }

    public required int ExtraBreakPenalty { get; set; }

    /// <summary>
    /// TableName is required
    /// Date is required
    /// EarlyPenalty should be between 1 and 5
    /// LatePenalty should be between 1 and 3
    /// MissedPenalty should be between 20 and 100
    /// </summary>
    /// <returns></returns>
    public List<ValidationResult> Validate()
    {
        var results = new List<ValidationResult>();

        if (string.IsNullOrEmpty(TableName))
        {
            results.Add(new ValidationResult("TableName is required", [nameof(TableName)]));
        }

        if (Date == default)
        {
            results.Add(new ValidationResult("Date is required", [nameof(Date)]));
        }

        if(Participants == 0)
        {
            results.Add(new ValidationResult("Participants should be greater than 0", [nameof(Participants)]));
        }

        if (EarlyPenalty is < 1 or > 5)
        {
            results.Add(new ValidationResult("EarlyPenalty should be between 1 and 5", [nameof(EarlyPenalty)]));
        }

        if (LatePenalty is < 1 or > 3)
        {
            results.Add(new ValidationResult("LatePenalty should be between 1 and 3", [nameof(LatePenalty)]));
        }

        if (MissedPenalty is < 30 or > 100)
        {
            results.Add(new ValidationResult("MissedPenalty should be between 30 and 100", [nameof(MissedPenalty)]));
        }

        if(ExtraBreakPenalty is < 0 or > 2)
        {
            results.Add(new ValidationResult("ExtraBreakPenalty should be between 0 and 2", [nameof(ExtraBreakPenalty)]));
        }

        return results;
    }
}