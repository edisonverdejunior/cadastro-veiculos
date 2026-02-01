using CadastroVeiculo.Domain.Entities;

namespace CadastroVeiculo.Domain.Interface.Service
{
    public interface IUsuarioService : IService<Usuario>
    {
        Task<Usuario?> ObterPorLoginAsync(string login);
        Task<bool> ExisteComLoginAsync(string login);
    }
}
