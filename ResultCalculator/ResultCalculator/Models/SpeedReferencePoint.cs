using System.ComponentModel.DataAnnotations;

internal class SpeedReferencePoint
{
    public required string Reference { get; set; }
    public double FromKM { get; set; }
    public double ToKM { get; set; }
    public double AverageSpeed { get; set; }

    /// <summary>
    /// Reference is required
    /// FromKM cannot be negative
    /// ToKM must be greater than FromKM
    /// AverageSpeed must be positive
    /// </summary>
    /// <returns></returns>
    public List<ValidationResult> Validate()
    {
        var results = new List<ValidationResult>();

        if(string.IsNullOrEmpty(Reference))
        {
            results.Add(new ValidationResult("Reference is required", [nameof(Reference)]));
        }

        if(FromKM is < 0 or > 100)
        {
            results.Add(new ValidationResult("FromKM should be non zero and less than 100", [nameof(FromKM)]));
        }

        if(ToKM <= FromKM)
        {
            results.Add(new ValidationResult("ToKM must be greater than FromKM", [nameof(ToKM)]));
        }

        if(AverageSpeed <= 0)
        {
            results.Add(new ValidationResult("AverageSpeed must be positive", [nameof(AverageSpeed)]));
        }
        
        return results;        
    }
}
