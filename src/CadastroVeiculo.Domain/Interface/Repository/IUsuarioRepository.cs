using CadastroVeiculo.Domain.Entities;

namespace CadastroVeiculo.Domain.Interface.Repository
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario?> ObterPorLoginAsync(string login);
        Task<bool> ExisteComLoginAsync(string login);
    }
}