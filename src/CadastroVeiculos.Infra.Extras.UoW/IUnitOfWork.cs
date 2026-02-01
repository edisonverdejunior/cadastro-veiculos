namespace CadastroVeiculos.Infra.Extras.UoW
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}
