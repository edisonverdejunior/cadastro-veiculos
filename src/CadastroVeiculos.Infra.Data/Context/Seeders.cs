using CadastroVeiculo.Domain.Entities;
using CadastroVeiculo.Domain.Enums;

namespace CadastroVeiculos.Infra.Data.Context
{
    public static class Seeders
    {
        public static void SeedInMemoryData(this CadastroVeiculosContext context)
        {
            SeedUsuarios(context);
            SeedVeiculos(context);

            context.SaveChanges();
        }

        private static void SeedVeiculos(CadastroVeiculosContext context)
        {
            var veiculos = new List<Veiculo>
            {
                new() 
                {
                    Descricao = "Supra 1998",
                    Marca = Marca.Toyota,
                    Modelo = "Supra",
                    Opcionais = "Ar condicionado, Direção hidráulica",
                    Valor = 85000
                },
                new()
                {
                    Descricao = "GT-R (R35)",
                    Marca = Marca.Nissan,
                    Modelo = "GT-R (R35)",
                    Opcionais = "Ar condicionado, Direção hidráulica, Freio ABS",
                    Valor = 1000000
                },
                new() 
                {
                    Descricao = "HRV EXL",
                    Marca = Marca.Honda,
                    Modelo = "HRV",
                    Opcionais = "Airbag",
                    Valor = 90000
                },
                new()
                {
                    Descricao = "Fusca",
                    Marca = Marca.Volkswagen,
                    Modelo = "Fusca 1300",
                    Opcionais = "Completo",
                    Valor = 10000
                },
                new()
                {
                    Descricao = "Variant",
                    Marca = Marca.Volkswagen,
                    Modelo = "Variant 1300",
                    Opcionais = "Sem opcionais",
                    Valor = 5000
                }
            };

            context.Veiculos.AddRange(veiculos);
        }

        private static void SeedUsuarios(CadastroVeiculosContext context)
        {
            if (context.Usuarios.Any())
                return;

            var senhaAdmin = BCrypt.Net.BCrypt.HashPassword("Admin@123");

            var usuarioAdmin = new Usuario
            {
                Nome = "Administrador",
                Login = "admin",
                Senha = senhaAdmin,
                Ativo = true
            };

            var usuarioUser = new Usuario
            {
                Nome = "Usuário",
                Login = "usuario",
                Senha = BCrypt.Net.BCrypt.HashPassword("User@123"),
                Ativo = true
            };

            context.Usuarios.AddRange(usuarioAdmin, usuarioUser);
        }
    }
}
