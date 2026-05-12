using DNAAnalysis.Domain.Entities.NutritionModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNAAnalysis.Persistence.Data.Configurations.Nutrition;

public class MealSuggestionConfiguration : IEntityTypeConfiguration<MealSuggestion>
{
    public void Configure(EntityTypeBuilder<MealSuggestion> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Calories)
            .IsRequired();

        builder.Property(x => x.MealType)
            .IsRequired();

        builder.Property(x => x.ProteinGrams)
            .IsRequired();

        builder.Property(x => x.CarbsGrams)
            .IsRequired();

        builder.Property(x => x.FatGrams)
            .IsRequired();
    }
}