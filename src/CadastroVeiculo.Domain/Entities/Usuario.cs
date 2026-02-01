namespace CadastroVeiculo.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public string Nome { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
    }
}
