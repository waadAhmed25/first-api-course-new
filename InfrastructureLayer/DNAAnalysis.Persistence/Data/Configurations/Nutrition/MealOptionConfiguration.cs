using DNAAnalysis.Domain.Entities.NutritionModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNAAnalysis.Persistence.Data.Configurations.Nutrition;

public class MealOptionConfiguration : IEntityTypeConfiguration<MealOption>
{
    public void Configure(EntityTypeBuilder<MealOption> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(x => x.MealSuggestion)
            .WithMany(x => x.Options)
            .HasForeignKey(x => x.MealSuggestionId);
    }
}