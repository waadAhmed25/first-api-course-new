using DNAAnalysis.Domain.Entities;
using DNAAnalysis.Shared.Enums;

namespace DNAAnalysis.Domain.Entities.NutritionModule;

public class NutritionProfile : BaseEntity<int>
{
    public string UserId { get; set; } = null!;

    public double Weight { get; set; }

    public double Height { get; set; }

    public int Age { get; set; }

    public string Gender { get; set; } = null!;

public string Activity { get; set; } = null!;

public string Status { get; set; } = null!;

public bool IncludeNightSnack { get; set; }

    public NutritionPlan? NutritionPlan { get; set; }
}