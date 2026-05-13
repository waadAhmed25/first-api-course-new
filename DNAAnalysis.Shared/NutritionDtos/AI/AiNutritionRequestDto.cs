// AiNutritionRequestDto.cs
// تم حذف ActivityLevel و HealthCondition
// وتم استبدالهم بـ Activity و Status

namespace DNAAnalysis.Shared.NutritionDtos.AI;

public class AiNutritionRequestDto
{
    public double Weight { get; set; }

    public double Height { get; set; }

    public int Age { get; set; }

    public string Gender { get; set; } = null!;

    public string Activity { get; set; } = null!;

    public string Status { get; set; } = null!;

    public bool IncludeNightSnack { get; set; }
}