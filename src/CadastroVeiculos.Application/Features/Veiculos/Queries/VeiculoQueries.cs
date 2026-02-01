using MediatR;

namespace CadastroVeiculos.Application.Features.Veiculos.Queries;

public class ObterVeiculoPorIdQuery : IRequest<ObterVeiculoResponse?>
{
    public Guid Id { get; set; }
}

public class ListarVeiculosQuery : IRequest<IEnumerable<ListarVeiculoResponse>>
{
}

public class ObterVeiculoResponse
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int Marca { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public string? Opcionais { get; set; }
    public decimal? Valor { get; set; }
}

public class ListarVeiculoResponse
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int Marca { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public string? Opcionais { get; set; }
    public decimal? Valor { get; set; }
}