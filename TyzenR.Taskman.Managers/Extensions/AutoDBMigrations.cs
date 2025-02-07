
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TyzenR.Taskman.Entity;

namespace TyzenR.Investor.Managers;
public static class AutoDBMigrations
{
    // Apply Auto DB Migration 
    public static WebApplication UseAutoDBMigrations(this WebApplication app )
    {
        using (var scope = app.Services.CreateScope())
        using (var dbContext = scope.ServiceProvider.GetRequiredService<EntityContext>())
        {
            // Auto DB Migration
            dbContext.Database.EnsureCreated();
            /*if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.Migrate();
            }*/

        }
        return app;
    }
}