using CadastroVeiculo.Domain.Entities;
using CadastroVeiculo.Domain.Interface.Service;
using CadastroVeiculos.Application.Features.Usuarios.Commands;
using CadastroVeiculos.Infra.Extras.UoW;
using FluentValidation;
using MediatR;

namespace CadastroVeiculos.Application.Features.Usuarios.Handlers;

public class CadastrarUsuarioHandler : IRequestHandler<CadastrarUsuarioCommand, CadastrarUsuarioResponse>
{
    private readonly IUsuarioService _usuarioService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<(string nome, string login, string senha)> _validator;

    public CadastrarUsuarioHandler(IUsuarioService usuarioService, IUnitOfWork unitOfWork, IValidator<(string nome, string login, string senha)> validator)
    {
        _usuarioService = usuarioService;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CadastrarUsuarioResponse> Handle(CadastrarUsuarioCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync((request.Nome, request.Login, request.Senha), cancellationToken: cancellationToken);

        var loginJaExiste = await _usuarioService.ExisteComLoginAsync(request.Login);
        if (loginJaExiste)
            throw new ValidationException("Login já existe");

        var usuario = new Usuario
        {
            Nome = request.Nome,
            Login = request.Login,
            Senha = BCrypt.Net.BCrypt.HashPassword(request.Senha)
        };

        var usuarioCriado = await _usuarioService.AdicionarAsync(usuario);
        await _unitOfWork.CommitAsync();

        return new CadastrarUsuarioResponse
        {
            Id = usuarioCriado.Id,
            Nome = usuarioCriado.Nome,
            Login = usuarioCriado.Login
        };
    }
}

public class AtualizarUsuarioHandler : IRequestHandler<AtualizarUsuarioCommand, AtualizarUsuarioResponse>
{
    private readonly IUsuarioService _usuarioService;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarUsuarioHandler(IUsuarioService usuarioService, IUnitOfWork unitOfWork)
    {
        _usuarioService = usuarioService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AtualizarUsuarioResponse> Handle(AtualizarUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioService.ObterPorIdAsync(request.Id);
        if (usuario == null)
            throw new KeyNotFoundException($"Usuário com id {request.Id} não encontrado");

        usuario.Nome = request.Nome;
        var usuarioAtualizado = await _usuarioService.AtualizarAsync(usuario);

        await _unitOfWork.CommitAsync();

        return new AtualizarUsuarioResponse
        {
            Id = usuarioAtualizado.Id,
            Nome = usuarioAtualizado.Nome
        };
    }
}

public class ExcluirUsuarioHandler : IRequestHandler<ExcluirUsuarioCommand, bool>
{
    private readonly IUsuarioService _usuarioService;
    private readonly IUnitOfWork _unitOfWork;

    public ExcluirUsuarioHandler(IUsuarioService usuarioService, IUnitOfWork unitOfWork)
    {
        _usuarioService = usuarioService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ExcluirUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioService.ObterPorIdAsync(request.Id);
        if (usuario == null)
            throw new KeyNotFoundException($"Usuário com id {request.Id} não encontrado");

        if (usuario.Login == "admin")
        {
            throw new InvalidOperationException($"Não é possível excluir o usuário 'admin'");
        }

        await _usuarioService.Delete(usuario);
        await _unitOfWork.CommitAsync();
        return true;
    }
}