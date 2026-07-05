using CadastroVeiculo.Domain.Interface.Repository;
using CadastroVeiculo.Domain.Interface.Service;
using CadastroVeiculo.Domain.Service;
using CadastroVeiculos.Application;
using CadastroVeiculos.Application.Validators;
using CadastroVeiculos.Infra.Data.Context;
using CadastroVeiculos.Infra.Data.Repository;
using CadastroVeiculos.Infra.Extras.UoW;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CadastroVeiculos.Infra.Extras.IoC
{
    public static class BootStrapper
    {
        public static void RegisterAllDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            RegisterContext(services, configuration);
            RegisterDomainServices(services);
            RegisterRepositories(services);
            RegisterValidators(services);
            RegisterMediatRHandlers(services);
        }

        private static void RegisterDomainServices(IServiceCollection services)
        {
            services.AddScoped<IVeiculoService, VeiculoService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMfaService, MfaService>();
            services.AddScoped<IMfaRecoveryCodeService, MfaRecoveryCodeService>();
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IVeiculoRepository, VeiculoRepository>();
            services.AddScoped<IMfaRecoveryCodeRepository, MfaRecoveryCodeRepository>();
        }

        private static void RegisterContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CadastroVeiculosContext>(options => 
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }

        private static void RegisterValidators(IServiceCollection services)
        {
            services.AddScoped<IValidator<(string nome, string login, string senha)>, CadastrarUsuarioValidator>();
            services.AddScoped<IValidator<(string descricao, int marca, string modelo, string? opcionais, decimal? valor)>, CadastrarVeiculoValidator>();
        }

        private static void RegisterMediatRHandlers(IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining(typeof(AssemblyReference));
            });
        }
    }
}
