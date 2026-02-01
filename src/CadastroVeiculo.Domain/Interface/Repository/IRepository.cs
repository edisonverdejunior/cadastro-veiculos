namespace CadastroVeiculo.Domain.Interface.Repository
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Task<TEntity> AdicionarAsync(TEntity obj);
        Task<TEntity> AtualizarAsync(TEntity obj);
        Task<TEntity?> ObterPorIdAsync(Guid? id);
        Task<IQueryable<TEntity>> ObterTodos();
        Task Delete(TEntity entity);
    }
}
