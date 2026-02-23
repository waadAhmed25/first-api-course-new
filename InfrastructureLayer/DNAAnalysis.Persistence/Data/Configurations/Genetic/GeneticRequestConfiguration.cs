using DNAAnalysis.Domain.Entities.GeneticModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNAAnalysis.Persistence.Configurations.Genetic
{
    public class GeneticRequestConfiguration : IEntityTypeConfiguration<GeneticRequest>
    {
        public void Configure(EntityTypeBuilder<GeneticRequest> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FatherFilePath)
                   .IsRequired();

            builder.Property(x => x.MotherFilePath)
                   .IsRequired();

            builder.Property(x => x.UserId)
                   .IsRequired();

            

            builder.HasOne(x => x.Result)
                   .WithOne(r => r.GeneticRequest)
                   .HasForeignKey<GeneticResult>(r => r.GeneticRequestId);
        }
    }
}