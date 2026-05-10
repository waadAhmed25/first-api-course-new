using DNAAnalysis.Domain.Entities.GeneticModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNAAnalysis.Persistence.Configurations.Genetic
{
    public class GeneticResultConfiguration 
        : IEntityTypeConfiguration<GeneticResult>
    {
        public void Configure(EntityTypeBuilder<GeneticResult> builder)
        {
            builder.HasKey(x => x.Id);

            // ✅ Raw JSON Response فقط
            builder.Property(x => x.RawAiResponse)
                   .IsRequired();

            // ✅ One-To-One Relation
            builder.HasOne(x => x.GeneticRequest)
                   .WithOne(x => x.Result)
                   .HasForeignKey<GeneticResult>(
                        x => x.GeneticRequestId);
        }
    }
}