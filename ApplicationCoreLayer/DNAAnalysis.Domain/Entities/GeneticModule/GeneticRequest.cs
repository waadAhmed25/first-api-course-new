using DNAAnalysis.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DNAAnalysis.Domain.Entities.GeneticModule
{
    public class GeneticRequest : BaseEntity
    {
        public string FatherFilePath { get; set; }
        public string MotherFilePath { get; set; }
        public string? ChildFilePath { get; set; }

        public bool IsProcessed { get; set; } = false;

        // 🔹 Foreign Key
        public string UserId { get; set; }

        // 🔹 Navigation Property
       // public ApplicationUser User { get; set; }

        // 🔹 One-To-One Result
        public GeneticResult? Result { get; set; }
    }
}