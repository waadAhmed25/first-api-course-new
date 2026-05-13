using System.Text.Json.Serialization;

namespace DNAAnalysis.Shared.NutritionDtos.AI;

public class AiMealDto
{
    [JsonPropertyName("calories")]
    public int Calories { get; set; }

    [JsonPropertyName("macros")]
    public AiMacrosDto Macros { get; set; } = null!;

    [JsonPropertyName("options")]
    public List<string> Options { get; set; }
        = new();
}