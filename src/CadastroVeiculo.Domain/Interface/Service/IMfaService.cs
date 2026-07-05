namespace CadastroVeiculo.Domain.Interface.Service
{
    /// <summary>
    /// Operações criptográficas de MFA/TOTP. Não acessa o banco — apenas gera/valida
    /// segredos, códigos e faz a proteção do segredo em repouso.
    /// </summary>
    public interface IMfaService
    {
        /// <summary>Gera um novo segredo TOTP (Base32) e a URI otpauth:// para o QR Code.</summary>
        MfaSetupResult GenerateSetup(string login);

        /// <summary>Cifra o segredo Base32 para persistência.</summary>
        string Protect(string plainSecret);

        /// <summary>Decifra o segredo persistido, retornando o Base32 original.</summary>
        string Unprotect(string protectedSecret);

        /// <summary>Valida um código TOTP de 6 dígitos contra o segredo Base32 (com janela p/ clock drift).</summary>
        bool ValidateCode(string plainSecret, string code);

        /// <summary>Gera N códigos de recuperação em texto puro (exibidos uma única vez).</summary>
        IReadOnlyList<string> GenerateRecoveryCodes(int count = 10);

        /// <summary>Hash de um código de recuperação para persistência.</summary>
        string HashRecoveryCode(string code);

        /// <summary>Verifica um código de recuperação informado contra o hash persistido.</summary>
        bool VerifyRecoveryCode(string code, string codeHash);
    }

    public class MfaSetupResult
    {
        /// <summary>Segredo TOTP em Base32 (texto puro — nunca persistir diretamente).</summary>
        public string PlainSecret { get; set; } = string.Empty;

        /// <summary>URI otpauth:// usada pelo front para renderizar o QR Code.</summary>
        public string OtpAuthUri { get; set; } = string.Empty;
    }
}
