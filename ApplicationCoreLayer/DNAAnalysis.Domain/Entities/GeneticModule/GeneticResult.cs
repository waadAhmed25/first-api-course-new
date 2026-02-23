using DNAAnalysis.Domain.Entities;

namespace DNAAnalysis.Domain.Entities.GeneticModule
{
    public class GeneticResult : BaseEntity
    {
        public string FatherStatus { get; set; }

        public string MotherStatus { get; set; }

        public string? ChildStatus { get; set; }

        public string MessageToPatient { get; set; }

        public string Advice { get; set; }

        // 🔹 Foreign Key
        public int GeneticRequestId { get; set; }

        public GeneticRequest GeneticRequest { get; set; }
    }
}