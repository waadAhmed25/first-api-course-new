namespace DNAAnalysis.Domain.Entities.GeneticModule;

public class GeneticResult : BaseEntity<int>
{
   public string RawAiResponse { get; set; } = default!;

    public int GeneticRequestId { get; set; }
    public GeneticRequest GeneticRequest { get; set; } = default!;
}