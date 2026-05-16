using CadastroVeiculo.Domain.Entities;
using CadastroVeiculo.Domain.Interface.Service;
using CadastroVeiculos.Application.Features.Veiculos.Commands;
using CadastroVeiculos.Infra.Extras.UoW;
using FluentValidation;
using MediatR;

namespace CadastroVeiculos.Application.Features.Veiculos.Handlers;

public class AdicionarVeiculoHandler : IRequestHandler<AdicionarVeiculoCommand, AdicionarVeiculoResponse>
{
    private readonly IVeiculoService _veiculoService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<(string descricao, int marca, string modelo, string? opcionais, decimal? valor)> _validator;

    public AdicionarVeiculoHandler(IVeiculoService veiculoService, IUnitOfWork unitOfWork, IValidator<(string descricao, int marca, string modelo, string? opcionais, decimal? valor)> validator)
    {
        _veiculoService = veiculoService;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<AdicionarVeiculoResponse> Handle(AdicionarVeiculoCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(
            (request.Descricao, (int)request.Marca, request.Modelo, request.Opcionais, request.Valor),
            cancellationToken: cancellationToken);

        var veiculo = new Veiculo
        {
            Descricao = request.Descricao,
            Marca = request.Marca,
            Modelo = request.Modelo,
            Opcionais = request.Opcionais,
            Valor = request.Valor
        };

        var veiculoCriado = await _veiculoService.AdicionarAsync(veiculo);

        await _unitOfWork.CommitAsync();

        return new AdicionarVeiculoResponse
        {
            Id = veiculoCriado.Id,
            Descricao = veiculoCriado.Descricao,
            Marca = (int)veiculoCriado.Marca,
            Modelo = veiculoCriado.Modelo
        };
    }
}

public class AtualizarVeiculoHandler : IRequestHandler<AtualizarVeiculoCommand, AtualizarVeiculoResponse>
{
    private readonly IVeiculoService _veiculoService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<(string descricao, int marca, string modelo, string? opcionais, decimal? valor)> _validator;

    public AtualizarVeiculoHandler(IVeiculoService veiculoService, IUnitOfWork unitOfWork, IValidator<(string descricao, int marca, string modelo, string? opcionais, decimal? valor)> validator)
    {
        _veiculoService = veiculoService;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<AtualizarVeiculoResponse> Handle(AtualizarVeiculoCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(
            (request.Descricao, (int)request.Marca, request.Modelo, request.Opcionais, request.Valor),
            cancellationToken: cancellationToken);

        var veiculo = await _veiculoService.ObterPorIdAsync(request.Id);
        if (veiculo == null)
            throw new KeyNotFoundException($"Veículo com id {request.Id} não encontrado");

        veiculo.Descricao = request.Descricao;
        veiculo.Marca = request.Marca;
        veiculo.Modelo = request.Modelo;
        veiculo.Opcionais = request.Opcionais;
        veiculo.Valor = request.Valor;

        var veiculoAtualizado = await _veiculoService.AtualizarAsync(veiculo);
    
        await _unitOfWork.CommitAsync();

        return new AtualizarVeiculoResponse
        {
            Id = veiculoAtualizado.Id,
            Descricao = veiculoAtualizado.Descricao,
            Marca = (int)veiculoAtualizado.Marca,
            Modelo = veiculoAtualizado.Modelo
        };
    }
}

public class ExcluirVeiculoHandler : IRequestHandler<ExcluirVeiculoCommand, bool>
{
    private readonly IVeiculoService _veiculoService;
    private readonly IUnitOfWork _unitOfWork;

    public ExcluirVeiculoHandler(IVeiculoService veiculoService, IUnitOfWork unitOfWork)
    {
        _veiculoService = veiculoService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ExcluirVeiculoCommand request, CancellationToken cancellationToken)
    {
        var veiculo = await _veiculoService.ObterPorIdAsync(request.Id);
        if (veiculo == null)
            throw new KeyNotFoundException($"Veículo com id {request.Id} não encontrado");

        await _veiculoService.Delete(veiculo);
        await _unitOfWork.CommitAsync();

        return true;
    }
}