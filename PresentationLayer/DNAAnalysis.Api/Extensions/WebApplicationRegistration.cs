using DNAAnalysis.Persistence.IdentityData.DbContext;
using Microsoft.EntityFrameworkCore;
using DNAAnalysis.Domain.Contracts;

using Microsoft.Extensions.DependencyInjection;



public static class WebApplicationRegistration{
public static async Task<WebApplication> MigrateIdentityDatabaseAsync(
    this WebApplication app)
{
    await using var scope = app.Services.CreateAsyncScope();

    var dbContextService =
        scope.ServiceProvider.GetRequiredService<StoreIdentityDbContext>();

    var pendingMigrations =
        await dbContextService.Database.GetPendingMigrationsAsync();

    if (pendingMigrations.Any())
        await dbContextService.Database.MigrateAsync();

    return app;
}
public static async Task<WebApplication> SeedIdentityDatabaseAsync(this WebApplication app)
{
    await using var scope = app.Services.CreateAsyncScope();

    var dataInitializer =
        scope.ServiceProvider.GetRequiredKeyedService<IDataInitializer>("Identity");

    await dataInitializer.InitializeAsync();

    return app;
}


}