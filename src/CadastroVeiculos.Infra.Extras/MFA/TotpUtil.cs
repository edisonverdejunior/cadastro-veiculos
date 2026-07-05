using System.Security.Cryptography;
using OtpNet;

namespace CadastroVeiculos.Infra.Extras.MFA
{
    /// <summary>
    /// Operações criptográficas puras de TOTP e códigos de recuperação (sem estado / sem banco).
    /// Espelha o padrão do <c>JwtService</c>: utilitário técnico em Infra.Extras consumido
    /// pelo serviço de domínio.
    /// </summary>
    public static class TotpUtil
    {
        private const int SecretSizeBytes = 20; // 160 bits (recomendado pela RFC 6238)
        private const int TotpDigits = 6;
        private const int TotpPeriodSeconds = 30;

        /// <summary>Gera um novo segredo TOTP aleatório codificado em Base32.</summary>
        public static string GenerateSecretBase32()
            => Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(SecretSizeBytes));

        /// <summary>Monta a URI otpauth:// consumida pelos apps autenticadores / QR Code.</summary>
        public static string BuildOtpAuthUri(string issuer, string login, string secretBase32)
        {
            var label = Uri.EscapeDataString($"{issuer}:{login}");
            var issuerEnc = Uri.EscapeDataString(issuer);
            return $"otpauth://totp/{label}?secret={secretBase32}&issuer={issuerEnc}" +
                   $"&algorithm=SHA1&digits={TotpDigits}&period={TotpPeriodSeconds}";
        }

        /// <summary>Valida um código TOTP aceitando ±1 passo (30s) para tolerar clock drift.</summary>
        public static bool ValidateTotp(string secretBase32, string code)
        {
            if (string.IsNullOrWhiteSpace(secretBase32) || string.IsNullOrWhiteSpace(code))
                return false;

            code = code.Trim();
            try
            {
                var totp = new Totp(Base32Encoding.ToBytes(secretBase32), step: TotpPeriodSeconds, totpSize: TotpDigits);
                return totp.VerifyTotp(code, out _, new VerificationWindow(previous: 1, future: 1));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Gera N códigos de recuperação legíveis (formato XXXXX-XXXXX).</summary>
        public static IReadOnlyList<string> GenerateRecoveryCodes(int count)
        {
            const string alphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789"; // sem chars ambíguos (0/O, 1/I/L)
            var codes = new List<string>(count);

            for (var i = 0; i < count; i++)
            {
                var chars = new char[10];
                for (var j = 0; j < chars.Length; j++)
                    chars[j] = alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];

                codes.Add($"{new string(chars, 0, 5)}-{new string(chars, 5, 5)}");
            }

            return codes;
        }

        public static string HashRecoveryCode(string code)
            => BCrypt.Net.BCrypt.HashPassword(Normalize(code));

        public static bool VerifyRecoveryCode(string code, string codeHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(Normalize(code), codeHash);
            }
            catch
            {
                return false;
            }
        }

        private static string Normalize(string code)
            => code.Trim().ToUpperInvariant().Replace(" ", string.Empty);
    }
}
