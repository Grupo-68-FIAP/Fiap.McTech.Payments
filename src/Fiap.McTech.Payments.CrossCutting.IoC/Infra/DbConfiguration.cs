using Fiap.McTech.Payments.CrossCutting.Exceptions;
using Fiap.McTech.Payments.Domain.Interfaces.Repositories;
using Fiap.McTech.Payments.Infra.Context;
using Fiap.McTech.Payments.Infra.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fiap.McTech.Payments.CrossCutting.IoC.Infra.Context
{
    public static class DbConfiguration
    {
        public static void ConfigureSqlServer(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                    ?? configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new DatabaseException("Database is not configured. Please inform your connection string.");

                services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error on database condigure.", ex);
            }
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPaymentsRepository, PaymentsRepository>();
        }

        public static void McTechDatabaseInitialize(this IServiceScope scope)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataContext>>();
            logger.LogInformation("Preparing database.");

            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            const int maxRetryAttempts = 3;
            var tryCount = 0;
            var dbConnection = false;

            do
            {
                try
                {
                    dbContext.Database.ExecuteSqlRaw("SELECT 1;");
                    dbConnection = true;
                }
                catch (SqlException ex)
                {
                    if (ex.ClientConnectionId == Guid.Empty)
                    {
                        tryCount++;
                        logger.LogWarning(ex, "Attempt {TryCount} of {MaxRetryAttempts}: Database connection failed. Retrying in 10 second...", tryCount, maxRetryAttempts);
                        Task.Delay(10000).Wait();
                    }
                    else
                    {
                        dbConnection = true;
                    }
                }
            } while (!dbConnection && tryCount < maxRetryAttempts);
            if (!dbConnection)
            {
                logger.LogError("Database connection failed after {MaxRetryAttempts} attempts. Exiting application.", maxRetryAttempts);
                throw new InvalidOperationException("Database connection failed.");
            }

            if (!dbContext.Database.CanConnect())
            {
                logger.LogWarning("Database {DbName} not found! Creating the database.", dbContext.Database.GetDbConnection().Database);
                dbContext.Database.Migrate();
            }

            var pendingMigrations = dbContext.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                logger.LogWarning("There are {Count} migrations that haven't been run yet. Updating the database.", pendingMigrations.Count());
                dbContext.Database.Migrate();
            }

            logger.LogInformation("Database is prepared.");
        }
    }
}
