namespace CadastroVeiculo.Domain.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            Id = new Guid();
        }

        public Guid Id { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
    }
}
