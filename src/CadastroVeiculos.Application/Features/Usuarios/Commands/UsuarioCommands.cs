using MediatR;

namespace CadastroVeiculos.Application.Features.Usuarios.Commands;

public class CadastrarUsuarioCommand : IRequest<CadastrarUsuarioResponse>
{
    public string Nome { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class CadastrarUsuarioResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
}

public class AtualizarUsuarioCommand : IRequest<AtualizarUsuarioResponse>
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class AtualizarUsuarioResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class ExcluirUsuarioCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
