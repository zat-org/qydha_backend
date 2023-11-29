using System.Reflection;
using DbUp;

namespace Qydha.Infrastructure;

public class DbMigrator
{
    public static void Migrate(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            // Handle migration failure (e.g., log error, throw exception)
            Console.WriteLine("Database migration failed:");
            Console.WriteLine(result.Error);
            throw new Exception("Database migration failed.");
        }
        else
        {
            Console.WriteLine("Database migration successful!");
        }
    }
}
