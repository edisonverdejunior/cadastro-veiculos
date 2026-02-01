using CadastroVeiculo.Domain.Interface.Repository;
using CadastroVeiculos.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CadastroVeiculos.Infra.Data.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected CadastroVeiculosContext Db;
        protected DbSet<TEntity> DbSet;

        public Repository(CadastroVeiculosContext context)
        {
            Db = context;
            DbSet = Db.Set<TEntity>();
        }

        public virtual async Task<TEntity> AdicionarAsync(TEntity obj)
        {
            var objreturn = await DbSet.AddAsync(obj);
            return objreturn.Entity;
        }

        public virtual async Task<TEntity> AtualizarAsync(TEntity obj)
        {
            Db.Entry(obj).State = EntityState.Modified;
            return DbSet.Update(obj).Entity;
        }

        public virtual async Task Delete(TEntity entity) => DbSet.Remove(entity);

        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }

        public virtual async Task<TEntity?> ObterPorIdAsync(Guid? id) => await DbSet.FindAsync(id);

        public virtual async Task<IQueryable<TEntity>> ObterTodos() => DbSet.AsNoTracking().AsQueryable();
    }
}
