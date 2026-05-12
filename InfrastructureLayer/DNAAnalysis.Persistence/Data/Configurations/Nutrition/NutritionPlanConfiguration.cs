using DNAAnalysis.Domain.Entities.NutritionModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNAAnalysis.Persistence.Data.Configurations.Nutrition;

public class NutritionPlanConfiguration : IEntityTypeConfiguration<NutritionPlan>
{
    public void Configure(EntityTypeBuilder<NutritionPlan> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Bmr)
            .IsRequired();

        builder.Property(x => x.Tdee)
            .IsRequired();

        builder.Property(x => x.FinalCaloriesGoal)
            .IsRequired();

        builder.Property(x => x.AiRawResponse)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.HasMany(x => x.MealSuggestions)
            .WithOne(x => x.NutritionPlan)
            .HasForeignKey(x => x.NutritionPlanId);

        builder
            .HasIndex(x => x.NutritionProfileId)
            .IsUnique();
    }
}