namespace CadastroVeiculo.Domain.Interface.Service
{
    public interface IAuthService
    {
        string GenerateToken(Guid userId, string login);
    }
}
