using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Factories;

/// <summary>
/// Usada exclusivamente pelo EF Core CLI (dotnet ef migrations add / update).
/// Lê as variáveis de ambiente do shell onde o comando é executado.
/// Antes de rodar comandos EF, exporte as variáveis no terminal:
///   export DB_SERVER=localhost DB_PORT=3306 DB_NAME=wbn_gestao_adm_dev DB_USER=root DB_PASSWORD=sua_senha
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var dbServer   = Environment.GetEnvironmentVariable("DB_SERVER")   ?? "localhost";
        var dbPort     = Environment.GetEnvironmentVariable("DB_PORT")     ?? "3306";
        var dbName     = Environment.GetEnvironmentVariable("DB_NAME")     ?? "wbn_gestao_adm_dev";
        var dbUser     = Environment.GetEnvironmentVariable("DB_USER")     ?? "root";
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD")
            ?? throw new InvalidOperationException(
                "A variável de ambiente DB_PASSWORD não foi definida. " +
                "Execute: export DB_PASSWORD=sua_senha antes de rodar comandos do EF.");

        var connectionString = $"Server={dbServer};Port={dbPort};Database={dbName};User={dbUser};Password={dbPassword};Allow User Variables=true;";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));

        return new AppDbContext(optionsBuilder.Options);
    }
}
