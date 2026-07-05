namespace CadastroVeiculo.Domain.Interface.Service
{
    public interface IAuthService
    {
        /// <summary>Token de acesso final (após senha, e após MFA quando habilitado).</summary>
        string GenerateToken(Guid userId, string login);

        /// <summary>
        /// Token curto e de escopo restrito (audience "mfa-pending") emitido após a senha
        /// quando o MFA está habilitado. Não é aceito por endpoints [Authorize] normais.
        /// </summary>
        string GeneratePreAuthToken(Guid userId, string login);

        /// <summary>Valida um pré-auth token e retorna o userId, ou null se inválido/expirado.</summary>
        Guid? ValidatePreAuthToken(string token);
    }
}
