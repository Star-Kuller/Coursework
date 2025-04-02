using FluentMigrator.Runner;
using Npgsql;

namespace Coursework.Database;

public static class MigrationRunner
{
    public static void RunMigrations(string? connectionString, ILogger logger)
    {
        if(string.IsNullOrWhiteSpace(connectionString))
        {
            logger.LogWarning("connectionString пуста, миграции не применены");
            return;
        };
        try
        {
            // Проверяем существование базы данных
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            var databaseName = builder.Database;

            using (var connection = new NpgsqlConnection(builder.ConnectionString.Replace($"Database={databaseName}", "Database=postgres")))
            {
                connection.Open();
                var command = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname='{databaseName}'", connection);
                var exists = command.ExecuteScalar() != null;

                if (!exists)
                {
                    logger.LogInformation("База данных {DatabaseName} не существует. Создаём её...", databaseName);
                    command.CommandText = $"CREATE DATABASE \"{databaseName}\"";
                    command.ExecuteNonQuery();
                }
            }
            
            var serviceProvider = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(MigrationRunner).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
        catch
        {
            logger.LogError("Ошибка применения миграций");
            throw;
        }
        
    }
}