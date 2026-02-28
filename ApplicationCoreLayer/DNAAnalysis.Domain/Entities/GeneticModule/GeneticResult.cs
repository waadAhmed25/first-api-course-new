using DNAAnalysis.Domain.Entities;

namespace DNAAnalysis.Domain.Entities.GeneticModule
{

    public class GeneticResult : BaseEntity<int>
    {
        public string FatherStatus { get; set; } = default!;

        public string MotherStatus { get; set; } = default!;

        public string? ChildStatus { get; set; }

        public string MessageToPatient { get; set; } = default!;

        public string Advice { get; set; }  = default!;

        // 🔹 Foreign Key
        public int GeneticRequestId { get; set; }

        public GeneticRequest GeneticRequest { get; set; }  = default!;
        public string? Probabilities { get; set; }
    }
}