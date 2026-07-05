namespace CadastroVeiculo.Domain.Entities
{
    /// <summary>
    /// Código de recuperação de uso único para login MFA. Armazenado apenas como hash (BCrypt).
    /// </summary>
    public class MfaRecoveryCode : BaseEntity
    {
        public Guid UsuarioId { get; set; }
        public string CodeHash { get; set; } = string.Empty;
        public DateTime? UsedAt { get; set; }

        public Usuario? Usuario { get; set; }

        public bool Utilizado => UsedAt.HasValue;
    }
}
