using CadastroVeiculos.Application.Features.Auth.AuthQueries;
using MediatR;

namespace CadastroVeiculos.Application.Features.Mfa.Commands;

// ---- Etapa 2 do login (validação do TOTP / recovery code) ----
public class MfaLoginCommand : IRequest<LoginResponse>
{
    public string MfaToken { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

// ---- Início do cadastro (gera segredo + QR) ----
public class SetupMfaCommand : IRequest<SetupMfaResponse>
{
    public Guid UsuarioId { get; set; }
}

public class SetupMfaResponse
{
    public string Secret { get; set; } = string.Empty;
    public string OtpAuthUri { get; set; } = string.Empty;
}

// ---- Confirmação/ativação do MFA ----
public class EnableMfaCommand : IRequest<RecoveryCodesResponse>
{
    public Guid UsuarioId { get; set; }
    public string Code { get; set; } = string.Empty;
}

public class RecoveryCodesResponse
{
    public IReadOnlyList<string> RecoveryCodes { get; set; } = new List<string>();
}

// ---- Desabilitar MFA (exige TOTP válido) ----
public class DisableMfaCommand : IRequest<bool>
{
    public Guid UsuarioId { get; set; }
    public string Code { get; set; } = string.Empty;
}

// ---- Regerar códigos de recuperação (exige TOTP válido) ----
public class RegenerateRecoveryCodesCommand : IRequest<RecoveryCodesResponse>
{
    public Guid UsuarioId { get; set; }
    public string Code { get; set; } = string.Empty;
}

// ---- Reset administrativo (recuperação por perda de app + códigos) ----
public class ResetMfaCommand : IRequest<bool>
{
    public string SolicitanteLogin { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
}

// ---- Consulta de status para a tela de configurações ----
public class MfaStatusQuery : IRequest<MfaStatusResponse>
{
    public Guid UsuarioId { get; set; }
}

public class MfaStatusResponse
{
    public bool MfaEnabled { get; set; }
    public int RecoveryCodesRestantes { get; set; }
}
