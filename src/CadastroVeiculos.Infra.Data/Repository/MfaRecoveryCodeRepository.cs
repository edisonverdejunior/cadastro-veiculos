using CadastroVeiculo.Domain.Entities;
using CadastroVeiculo.Domain.Interface.Repository;
using CadastroVeiculos.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CadastroVeiculos.Infra.Data.Repository
{
    public class MfaRecoveryCodeRepository(CadastroVeiculosContext context)
        : Repository<MfaRecoveryCode>(context), IMfaRecoveryCodeRepository
    {
        public async Task<List<MfaRecoveryCode>> ObterAtivosPorUsuarioAsync(Guid usuarioId)
            => await DbSet.Where(c => c.UsuarioId == usuarioId && c.UsedAt == null).ToListAsync();

        public async Task RemoverPorUsuarioAsync(Guid usuarioId)
        {
            var existentes = await DbSet.Where(c => c.UsuarioId == usuarioId).ToListAsync();
            DbSet.RemoveRange(existentes);
        }

        public async Task AdicionarRangeAsync(IEnumerable<MfaRecoveryCode> codes)
            => await DbSet.AddRangeAsync(codes);
    }
}
