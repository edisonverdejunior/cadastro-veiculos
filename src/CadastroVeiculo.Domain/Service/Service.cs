using CadastroVeiculo.Domain.Interface.Repository;
using CadastroVeiculo.Domain.Interface.Service;

namespace CadastroVeiculo.Domain.Service
{
    public class Service<TEntity>(IRepository<TEntity> repository) : IService<TEntity> where TEntity : class
    {
        private readonly IRepository<TEntity> _repository = repository;

        public async Task<TEntity> AdicionarAsync(TEntity obj) => await _repository.AdicionarAsync(obj);

        public async Task<TEntity> AtualizarAsync(TEntity obj)
            => await _repository.AtualizarAsync(obj);

        public async Task Delete(TEntity entity) => await _repository.Delete(entity);
        
        public void Dispose()
        {
            _repository.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<TEntity?> ObterPorIdAsync(Guid? id) => await _repository.ObterPorIdAsync(id);

        public Task<IQueryable<TEntity>> ObterTodos() => _repository.ObterTodos();
    }
}
