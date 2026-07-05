using CadastroVeiculo.Domain.Entities;

namespace CadastroVeiculo.Domain.Interface.Repository
{
    public interface IMfaRecoveryCodeRepository : IRepository<MfaRecoveryCode>
    {
        Task<List<MfaRecoveryCode>> ObterAtivosPorUsuarioAsync(Guid usuarioId);
        Task RemoverPorUsuarioAsync(Guid usuarioId);
        Task AdicionarRangeAsync(IEnumerable<MfaRecoveryCode> codes);
    }
}
