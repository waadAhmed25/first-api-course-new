namespace DNAAnalysis.Shared.GeneticResultDtos
{
    public class GeneticResultDto
    {
        public int Id { get; set; }
        

        public string FatherStatus { get; set; } = default!;
        public string MotherStatus { get; set; } = default!;
        public string? ChildStatus { get; set; }

        public string MessageToPatient { get; set; } = default!;
        public string Advice { get; set; } = default!;
    }
}