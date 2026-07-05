using CadastroVeiculo.Domain.Entities;

namespace CadastroVeiculo.Domain.Interface.Service
{
    public interface IMfaRecoveryCodeService : IService<MfaRecoveryCode>
    {
        Task<List<MfaRecoveryCode>> ObterAtivosPorUsuarioAsync(Guid usuarioId);
        Task RemoverPorUsuarioAsync(Guid usuarioId);
        Task AdicionarRangeAsync(IEnumerable<MfaRecoveryCode> codes);
    }
}
