using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CadastroVeiculos.Infra.Data.Context
{
    public class CadastroVeiculosContextFactory : IDesignTimeDbContextFactory<CadastroVeiculosContext>
    {
        public CadastroVeiculosContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CadastroVeiculosContext>();

            // Hardcoded connection string for dotnet ef migrations CLI only
            // Runtime connection string comes from appsettings.json via BootStrapper
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=cadastro_veiculos;Username=postgres;Password=postgres");

            return new CadastroVeiculosContext(optionsBuilder.Options);
        }
    }
}
