namespace DNAAnalysis.Shared.GeneticRequestDtos;

public class CreateGeneticRequestDto
{
    public string FatherFilePath { get; set; } = default!;
    public string MotherFilePath { get; set; } = default!;
    public string? ChildFilePath { get; set; }
}

