namespace DNAAnalysis.Shared.DrugDtos;

public class DrugInteractionDto
{
    public int Id { get; set; }

    public string Drug1 { get; set; } = string.Empty;
    public string Drug2 { get; set; } = string.Empty;

    public bool HasInteraction { get; set; }

    public string? Severity { get; set; }
    public string? Description { get; set; }
    public string UserId { get; set; } = default!;
}