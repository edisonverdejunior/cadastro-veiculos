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

    /// <summary>Quando true, a senha foi aceita mas o 2º fator (TOTP) ainda é necessário.</summary>
    public bool MfaRequired { get; set; }

    /// <summary>Token curto de escopo restrito usado na 2ª etapa. Preenchido apenas quando MfaRequired.</summary>
    public string? MfaToken { get; set; }
}