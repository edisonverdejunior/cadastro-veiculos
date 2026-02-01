using MediatR;

namespace CadastroVeiculos.Application.Features.Auth.AuthQueries;

public class LoginQuery : IRequest<LoginResponse>
{
    public string Login { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresIn { get; set; }
    public string TokenType { get; set; } = "Bearer";
}