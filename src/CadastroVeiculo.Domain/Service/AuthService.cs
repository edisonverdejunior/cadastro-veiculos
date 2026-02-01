using CadastroVeiculo.Domain.Interface.Service;
using CadastroVeiculos.Infra.Extras.JWT;

namespace CadastroVeiculo.Domain.Service
{
    public class AuthService : IAuthService
    {
        public string GenerateToken(Guid userId, string login)
            => JwtService.GenerateToken(userId, login);
    }
}
