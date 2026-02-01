using CadastroVeiculo.Domain.Enums;
using MediatR;

namespace CadastroVeiculos.Application.Features.Veiculos.Commands;


public class AdicionarVeiculoCommand : IRequest<AdicionarVeiculoResponse>
{
    public string Descricao { get; set; } = string.Empty;
    public Marca Marca { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public string? Opcionais { get; set; }
    public decimal? Valor { get; set; }
}

public class AdicionarVeiculoResponse
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int Marca { get; set; }
    public string Modelo { get; set; } = string.Empty;
}

public class AtualizarVeiculoCommand : IRequest<AtualizarVeiculoResponse>
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public Marca Marca { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public string? Opcionais { get; set; }
    public decimal? Valor { get; set; }
}

public class AtualizarVeiculoResponse
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int Marca { get; set; }
    public string Modelo { get; set; } = string.Empty;
}

public class ExcluirVeiculoCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}