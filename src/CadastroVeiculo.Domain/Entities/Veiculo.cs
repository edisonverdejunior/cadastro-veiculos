using CadastroVeiculo.Domain.Enums;

namespace CadastroVeiculo.Domain.Entities
{
    public class Veiculo : BaseEntity
    {
        public string Descricao { get; set; } = string.Empty;
        public Marca Marca { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string? Opcionais { get; set; }
        public decimal? Valor { get; set; }
    }
}
