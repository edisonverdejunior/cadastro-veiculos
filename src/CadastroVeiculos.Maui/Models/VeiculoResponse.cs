namespace CadastroVeiculos.Maui.Models;

public class VeiculoResponse
{
    public string Id { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int Marca { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public string? Opcionais { get; set; }
    public decimal? Valor { get; set; }

    public string MarcaNome => MarcaItem.ObterNome(Marca);
    public string ValorFormatado => Valor.HasValue ? Valor.Value.ToString("C2") : "-";
}
