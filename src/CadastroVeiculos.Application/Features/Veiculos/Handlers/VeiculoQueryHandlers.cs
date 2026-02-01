using CadastroVeiculo.Domain.Interface.Service;
using CadastroVeiculos.Application.Features.Veiculos.Queries;
using MediatR;

namespace CadastroVeiculos.Application.Features.Veiculos.Handlers;

public class ObterVeiculoPorIdHandler : IRequestHandler<ObterVeiculoPorIdQuery, ObterVeiculoResponse?>
{
    private readonly IVeiculoService _veiculoService;

    public ObterVeiculoPorIdHandler(IVeiculoService veiculoService)
    {
        _veiculoService = veiculoService;
    }

    public async Task<ObterVeiculoResponse?> Handle(ObterVeiculoPorIdQuery request, CancellationToken cancellationToken)
    {
        var veiculo = await _veiculoService.ObterPorIdAsync(request.Id);

        if (veiculo == null)
            return null;

        return new ObterVeiculoResponse
        {
            Id = veiculo.Id,
            Descricao = veiculo.Descricao,
            Marca = (int)veiculo.Marca,
            Modelo = veiculo.Modelo,
            Opcionais = veiculo.Opcionais,
            Valor = veiculo.Valor
        };
    }
}

public class ListarVeiculosHandler : IRequestHandler<ListarVeiculosQuery, IEnumerable<ListarVeiculoResponse>>
{
    private readonly IVeiculoService _veiculoService;

    public ListarVeiculosHandler(IVeiculoService veiculoService)
    {
        _veiculoService = veiculoService;
    }

    public async Task<IEnumerable<ListarVeiculoResponse>> Handle(ListarVeiculosQuery request, CancellationToken cancellationToken)
    {
        var veiculos = await _veiculoService.ObterTodos();

        return veiculos.Select(v => new ListarVeiculoResponse
        {
            Id = v.Id,
            Descricao = v.Descricao,
            Marca = (int)v.Marca,
            Modelo = v.Modelo,
            Opcionais = v.Opcionais,
            Valor = v.Valor
        });
    }
}