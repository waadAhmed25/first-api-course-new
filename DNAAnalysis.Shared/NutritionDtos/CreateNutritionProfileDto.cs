using DNAAnalysis.Shared.Enums;

namespace DNAAnalysis.Shared.NutritionDtos;

public class CreateNutritionProfileDto
{
    public double Weight { get; set; }

    public double Height { get; set; }

    public int Age { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string Activity { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public bool IncludeNightSnack { get; set; }
}