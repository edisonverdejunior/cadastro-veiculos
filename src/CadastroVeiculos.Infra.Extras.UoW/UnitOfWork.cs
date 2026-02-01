using CadastroVeiculos.Infra.Data.Context;

namespace CadastroVeiculos.Infra.Extras.UoW
{
    public class UnitOfWork(CadastroVeiculosContext context) : IUnitOfWork
    {
        private readonly CadastroVeiculosContext _context = context;

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
