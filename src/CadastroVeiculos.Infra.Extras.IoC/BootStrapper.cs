using CadastroVeiculo.Domain.Interface.Repository;
using CadastroVeiculo.Domain.Interface.Service;
using CadastroVeiculo.Domain.Service;
using CadastroVeiculos.Application.Validators;
using CadastroVeiculos.Infra.Data.Context;
using CadastroVeiculos.Infra.Data.Repository;
using CadastroVeiculos.Infra.Extras.UoW;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CadastroVeiculos.Infra.Extras.IoC
{
    public static class BootStrapper
    {
        public static void RegisterAllDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            RegisterContext(services);
            RegisterDomainServices(services);
            RegisterRepositories(services);
            RegisterVaidators(services);
        }

        private static void RegisterDomainServices(IServiceCollection services)
        {
            services.AddScoped<IVeiculoService, VeiculoService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IAuthService, AuthService>();
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IVeiculoRepository, VeiculoRepository>();
        }

        private static void RegisterContext(IServiceCollection services)
        {
            services.AddDbContext<CadastroVeiculosContext>(options => options.UseInMemoryDatabase("CadastroVeiculosDb"));
        }

        private static void RegisterVaidators(IServiceCollection services)
        {
            services.AddScoped<IValidator<(string nome, string login, string senha)>, CadastrarUsuarioValidator>();
            services.AddScoped<IValidator<(string descricao, int marca, string modelo, string? opcionais, decimal? valor)>, CadastrarVeiculoValidator>();
        }
    }
}
