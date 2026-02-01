using CadastroVeiculo.Domain.Entities;
using CadastroVeiculo.Domain.Interface.Repository;
using CadastroVeiculos.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CadastroVeiculos.Infra.Data.Repository
{
    public class UsuarioRepository(CadastroVeiculosContext context) : Repository<Usuario>(context), IUsuarioRepository
    {
        public async Task<Usuario?> ObterPorLoginAsync(string login)
             => await DbSet.FirstOrDefaultAsync(x => x.Login == login);

        public async Task<bool> ExisteComLoginAsync(string login)
            => await DbSet.AnyAsync(x => x.Login == login);
    }
}
