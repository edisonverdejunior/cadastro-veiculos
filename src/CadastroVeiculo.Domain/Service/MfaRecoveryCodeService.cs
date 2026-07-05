using CadastroVeiculo.Domain.Entities;
using CadastroVeiculo.Domain.Interface.Repository;
using CadastroVeiculo.Domain.Interface.Service;

namespace CadastroVeiculo.Domain.Service
{
    public class MfaRecoveryCodeService(IMfaRecoveryCodeRepository repository)
        : Service<MfaRecoveryCode>(repository), IMfaRecoveryCodeService
    {
        public async Task<List<MfaRecoveryCode>> ObterAtivosPorUsuarioAsync(Guid usuarioId)
            => await repository.ObterAtivosPorUsuarioAsync(usuarioId);

        public async Task RemoverPorUsuarioAsync(Guid usuarioId)
            => await repository.RemoverPorUsuarioAsync(usuarioId);

        public async Task AdicionarRangeAsync(IEnumerable<MfaRecoveryCode> codes)
            => await repository.AdicionarRangeAsync(codes);
    }
}
