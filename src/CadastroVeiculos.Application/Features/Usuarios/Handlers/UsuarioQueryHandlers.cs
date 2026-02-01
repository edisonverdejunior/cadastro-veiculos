using CadastroVeiculo.Domain.Interface.Service;
using CadastroVeiculos.Application.Features.Usuarios.Queries;
using MediatR;

namespace CadastroVeiculos.Application.Features.Usuarios.Handlers;

public class ObterUsuarioPorIdHandler : IRequestHandler<ObterUsuarioPorIdQuery, ObterUsuarioResponse?>
{
    private readonly IUsuarioService _usuarioService;

    public ObterUsuarioPorIdHandler(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    public async Task<ObterUsuarioResponse?> Handle(ObterUsuarioPorIdQuery request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioService.ObterPorIdAsync(request.Id);

        if (usuario == null)
            return null;

        return new ObterUsuarioResponse
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Login = usuario.Login
        };
    }
}

public class ListarUsuariosHandler : IRequestHandler<ListarUsuariosQuery, IEnumerable<ListarUsuarioResponse>>
{
    private readonly IUsuarioService _usuarioService;

    public ListarUsuariosHandler(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    public async Task<IEnumerable<ListarUsuarioResponse>> Handle(ListarUsuariosQuery request, CancellationToken cancellationToken)
    {
        var usuarios = await _usuarioService.ObterTodos();

        return usuarios.Select(u => new ListarUsuarioResponse
        {
            Id = u.Id,
            Nome = u.Nome,
            Login = u.Login
        });
    }
}