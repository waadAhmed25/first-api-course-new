using DNAAnalysis.Domain.Entities.GeneticModule;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using DNAAnalysis.Domain.Entities.DrugModule;
using DNAAnalysis.Domain.Entities.NutritionModule;
using DNAAnalysis.Domain.Entities.AlarmModule;

namespace DNAAnalysis.Persistence.Data.DBContexts
{
    public class DNAAnalysisDbContext : DbContext
    {
        public DNAAnalysisDbContext(DbContextOptions<DNAAnalysisDbContext> options)
            : base(options)
        {
        }

        // ✅ Genetic Module
        public DbSet<GeneticRequest> GeneticRequests { get; set; }
        public DbSet<GeneticResult> GeneticResults { get; set; }

        // ✅ Drug Module
        public DbSet<DrugInteraction> DrugInteractions { get; set; }
       
       // ✅ Alarm Module
        public DbSet<Reminder> Reminders { get; set; }
        
        // ✅ Nutrition Module
        public DbSet<NutritionProfile> NutritionProfiles { get; set; }
        public DbSet<NutritionPlan> NutritionPlans { get; set; }
        public DbSet<MealSuggestion> MealSuggestions { get; set; }

        // ✅ Apply Configurations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserMealSelection> UserMealSelections { get; set; }

    }
}