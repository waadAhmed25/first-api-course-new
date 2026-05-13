using System.Text.Json.Serialization;

namespace DNAAnalysis.Shared.NutritionDtos.AI;

public class AiMacrosDto
{
    [JsonPropertyName("protein")]
    public MacroValue Protein { get; set; } = null!;

    [JsonPropertyName("carbs")]
    public MacroValue Carbs { get; set; } = null!;

    [JsonPropertyName("fat")]
    public MacroValue Fat { get; set; } = null!;
}

public class MacroValue
{
    [JsonPropertyName("grams")]
    public double Grams { get; set; }
}