using DNAAnalysis.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using DNAAnalysis.Shared.Enums;


namespace DNAAnalysis.Domain.Entities.GeneticModule
{
    public class GeneticRequest : BaseEntity<int>
    {
        public string FatherFilePath { get; set; } = default!;
        public string MotherFilePath { get; set; } = default!;
        public string? ChildFilePath { get; set; }

       public RequestStatus Status { get; set; } = RequestStatus.Pending;

        // 🔹 Foreign Key
        public string UserId { get; set; } = default!;

        // 🔹 Navigation Property
       // public ApplicationUser User { get; set; }

        // 🔹 One-To-One Result
        public GeneticResult? Result { get; set; }
    }
}