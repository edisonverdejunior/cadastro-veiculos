using CadastroVeiculo.Domain.Interface.Service;
using CadastroVeiculos.Infra.Extras.Configurations;
using CadastroVeiculos.Infra.Extras.MFA;
using Microsoft.AspNetCore.DataProtection;

namespace CadastroVeiculo.Domain.Service
{
    public class MfaService : IMfaService
    {
        private const string ProtectorPurpose = "CadastroVeiculos.Mfa.Secret.v1";
        private readonly IDataProtector _protector;

        public MfaService(IDataProtectionProvider dataProtectionProvider)
        {
            _protector = dataProtectionProvider.CreateProtector(ProtectorPurpose);
        }

        public MfaSetupResult GenerateSetup(string login)
        {
            var secret = TotpUtil.GenerateSecretBase32();
            var issuer = Config.GetSectionValue("Jwt", "Issuer") ?? "CadastroVeiculos";

            return new MfaSetupResult
            {
                PlainSecret = secret,
                OtpAuthUri = TotpUtil.BuildOtpAuthUri(issuer, login, secret)
            };
        }

        public string Protect(string plainSecret) => _protector.Protect(plainSecret);

        public string Unprotect(string protectedSecret) => _protector.Unprotect(protectedSecret);

        public bool ValidateCode(string plainSecret, string code) => TotpUtil.ValidateTotp(plainSecret, code);

        public IReadOnlyList<string> GenerateRecoveryCodes(int count = 10)
            => TotpUtil.GenerateRecoveryCodes(count);

        public string HashRecoveryCode(string code) => TotpUtil.HashRecoveryCode(code);

        public bool VerifyRecoveryCode(string code, string codeHash) => TotpUtil.VerifyRecoveryCode(code, codeHash);
    }
}
