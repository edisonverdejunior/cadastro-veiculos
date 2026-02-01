using MediatR;

namespace CadastroVeiculos.Application.Features.Usuarios.Queries;

public class ObterUsuarioPorIdQuery : IRequest<ObterUsuarioResponse?>
{
    public Guid Id { get; set; }
}

public class ListarUsuariosQuery : IRequest<IEnumerable<ListarUsuarioResponse>>
{
}

public class ObterUsuarioResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
}

public class ListarUsuarioResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
}
