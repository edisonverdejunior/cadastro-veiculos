using MediatR;

namespace CadastroVeiculos.Application.Features.Veiculos.Queries;

public class ObterVeiculoPorIdQuery : IRequest<ObterVeiculoResponse?>
{
    public Guid Id { get; set; }
}

public class ListarVeiculosQuery : IRequest<IEnumerable<ListarVeiculoResponse>>
{

}

public class BuscarVeiculosQuery : IRequest<IEnumerable<ListarVeiculoResponse>>
{
    public string? Descricao { get; set; }
    public int? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? Opcionais { get; set; }
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