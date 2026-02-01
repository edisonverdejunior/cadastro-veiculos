using CadastroVeiculo.Domain.Entities;
using CadastroVeiculo.Domain.Interface.Repository;
using CadastroVeiculo.Domain.Interface.Service;

namespace CadastroVeiculo.Domain.Service
{
    public class UsuarioService(IUsuarioRepository repository) : Service<Usuario>(repository), IUsuarioService
    {
        public async Task<Usuario?> ObterPorLoginAsync(string login)
            => await repository.ObterPorLoginAsync(login);

        public async Task<bool> ExisteComLoginAsync(string login)
            => await repository.ExisteComLoginAsync(login);
    }
}
