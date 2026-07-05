namespace CadastroVeiculo.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public string Nome { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;

        // MFA (TOTP)
        public bool MfaEnabled { get; set; } = false;

        /// <summary>Segredo TOTP em Base32, cifrado em repouso via IDataProtector.</summary>
        public string? MfaSecret { get; set; }

        public DateTime? MfaEnrolledAt { get; set; }

        public ICollection<MfaRecoveryCode> MfaRecoveryCodes { get; set; } = new List<MfaRecoveryCode>();
    }
}
