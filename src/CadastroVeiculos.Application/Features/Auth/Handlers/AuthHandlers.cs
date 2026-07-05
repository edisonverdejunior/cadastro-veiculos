using CadastroVeiculo.Domain.Interface.Service;
using CadastroVeiculos.Application.Features.Auth.AuthQueries;
using FluentValidation;
using MediatR;

namespace CadastroVeiculos.Application.Features.Auth.Handlers;

public class LoginHandler : IRequestHandler<LoginQuery, LoginResponse>
{
    private readonly IUsuarioService _usuarioService;
    private readonly IAuthService _authService;

    public LoginHandler(IUsuarioService usuarioService, IAuthService authService)
    {
        _usuarioService = usuarioService;
        _authService = authService;
    }

    public async Task<LoginResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Senha))
            throw new ValidationException("Login e Senha são obrigatórios");

        var usuario = await _usuarioService.ObterPorLoginAsync(request.Login);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.Senha))
            throw new UnauthorizedAccessException("Login ou senha inválidos");

        // 2º fator habilitado: não emite o token final; devolve um pré-auth token curto.
        if (usuario.MfaEnabled)
        {
            return new LoginResponse
            {
                MfaRequired = true,
                MfaToken = _authService.GeneratePreAuthToken(usuario.Id, usuario.Login)
            };
        }

        var token = _authService.GenerateToken(usuario.Id, usuario.Login);

        return new LoginResponse
        {
            Token = token,
            ExpiresIn = DateTime.UtcNow.AddHours(1),
            TokenType = "Bearer"
        };
    }
}
