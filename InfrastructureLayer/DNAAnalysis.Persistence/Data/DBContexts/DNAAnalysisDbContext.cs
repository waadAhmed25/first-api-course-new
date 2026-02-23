using DNAAnalysis.Domain.Entities.GeneticModule;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DNAAnalysis.Persistence.Data.DBContexts
{
    public class DNAAnalysisDbContext : DbContext
    {
        public DNAAnalysisDbContext(DbContextOptions<DNAAnalysisDbContext> options)
            : base(options)
        {
        }

        // ✅ هنا بنعمل DbSet لكل Entity في الموديول
        public DbSet<GeneticRequest> GeneticRequests { get; set; }
        public DbSet<GeneticResult> GeneticResults { get; set; }

        // ✅ هنا بنخلي EF يشوف كل Configurations اللي عملناها
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}