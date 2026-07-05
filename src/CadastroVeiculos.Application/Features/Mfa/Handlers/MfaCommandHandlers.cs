using CadastroVeiculo.Domain.Entities;
using CadastroVeiculo.Domain.Interface.Service;
using CadastroVeiculos.Application.Features.Auth.AuthQueries;
using CadastroVeiculos.Application.Features.Mfa.Commands;
using CadastroVeiculos.Infra.Extras.UoW;
using FluentValidation;
using MediatR;

namespace CadastroVeiculos.Application.Features.Mfa.Handlers;

// ============ Etapa 2 do login ============
public class MfaLoginHandler : IRequestHandler<MfaLoginCommand, LoginResponse>
{
    private readonly IUsuarioService _usuarioService;
    private readonly IMfaService _mfaService;
    private readonly IMfaRecoveryCodeService _recoveryCodeService;
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;

    public MfaLoginHandler(IUsuarioService usuarioService, IMfaService mfaService,
        IMfaRecoveryCodeService recoveryCodeService, IAuthService authService, IUnitOfWork unitOfWork)
    {
        _usuarioService = usuarioService;
        _mfaService = mfaService;
        _recoveryCodeService = recoveryCodeService;
        _authService = authService;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResponse> Handle(MfaLoginCommand request, CancellationToken cancellationToken)
    {
        var userId = _authService.ValidatePreAuthToken(request.MfaToken)
            ?? throw new UnauthorizedAccessException("Sessão de MFA inválida ou expirada");

        var usuario = await _usuarioService.ObterPorIdAsync(userId);
        if (usuario is null || !usuario.MfaEnabled || string.IsNullOrEmpty(usuario.MfaSecret))
            throw new UnauthorizedAccessException("MFA não habilitado para este usuário");

        if (string.IsNullOrWhiteSpace(request.Code))
            throw new ValidationException("Código é obrigatório");

        var codigoValido = await ValidarCodigoAsync(usuario, request.Code);
        if (!codigoValido)
            throw new UnauthorizedAccessException("Código inválido");

        var token = _authService.GenerateToken(usuario.Id, usuario.Login);
        return new LoginResponse
        {
            Token = token,
            ExpiresIn = DateTime.UtcNow.AddHours(1),
            TokenType = "Bearer"
        };
    }

    /// <summary>Aceita TOTP; se falhar, tenta um código de recuperação (consumindo-o).</summary>
    private async Task<bool> ValidarCodigoAsync(Usuario usuario, string code)
    {
        var secret = _mfaService.Unprotect(usuario.MfaSecret!);
        if (_mfaService.ValidateCode(secret, code))
            return true;

        var ativos = await _recoveryCodeService.ObterAtivosPorUsuarioAsync(usuario.Id);
        var match = ativos.FirstOrDefault(c => _mfaService.VerifyRecoveryCode(code, c.CodeHash));
        if (match is null)
            return false;

        match.UsedAt = DateTime.UtcNow;
        match.DataAtualizacao = DateTime.UtcNow;
        await _recoveryCodeService.AtualizarAsync(match);
        await _unitOfWork.CommitAsync();
        return true;
    }
}

// ============ Início do cadastro ============
public class SetupMfaHandler : IRequestHandler<SetupMfaCommand, SetupMfaResponse>
{
    private readonly IUsuarioService _usuarioService;
    private readonly IMfaService _mfaService;
    private readonly IUnitOfWork _unitOfWork;

    public SetupMfaHandler(IUsuarioService usuarioService, IMfaService mfaService, IUnitOfWork unitOfWork)
    {
        _usuarioService = usuarioService;
        _mfaService = mfaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<SetupMfaResponse> Handle(SetupMfaCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioService.ObterPorIdAsync(request.UsuarioId)
            ?? throw new KeyNotFoundException("Usuário não encontrado");

        if (usuario.MfaEnabled)
            throw new InvalidOperationException("MFA já está habilitado. Desabilite antes de cadastrar novamente.");

        var setup = _mfaService.GenerateSetup(usuario.Login);

        usuario.MfaSecret = _mfaService.Protect(setup.PlainSecret);
        usuario.DataAtualizacao = DateTime.UtcNow;
        await _usuarioService.AtualizarAsync(usuario);
        await _unitOfWork.CommitAsync();

        return new SetupMfaResponse { Secret = setup.PlainSecret, OtpAuthUri = setup.OtpAuthUri };
    }
}

// ============ Ativação ============
public class EnableMfaHandler : IRequestHandler<EnableMfaCommand, RecoveryCodesResponse>
{
    private readonly IUsuarioService _usuarioService;
    private readonly IMfaService _mfaService;
    private readonly IMfaRecoveryCodeService _recoveryCodeService;
    private readonly IUnitOfWork _unitOfWork;

    public EnableMfaHandler(IUsuarioService usuarioService, IMfaService mfaService,
        IMfaRecoveryCodeService recoveryCodeService, IUnitOfWork unitOfWork)
    {
        _usuarioService = usuarioService;
        _mfaService = mfaService;
        _recoveryCodeService = recoveryCodeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<RecoveryCodesResponse> Handle(EnableMfaCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioService.ObterPorIdAsync(request.UsuarioId)
            ?? throw new KeyNotFoundException("Usuário não encontrado");

        if (usuario.MfaEnabled)
            throw new InvalidOperationException("MFA já está habilitado");

        if (string.IsNullOrEmpty(usuario.MfaSecret))
            throw new InvalidOperationException("Inicie o cadastro do MFA antes de ativá-lo");

        var secret = _mfaService.Unprotect(usuario.MfaSecret);
        if (!_mfaService.ValidateCode(secret, request.Code))
            throw new ValidationException("Código inválido");

        usuario.MfaEnabled = true;
        usuario.MfaEnrolledAt = DateTime.UtcNow;
        usuario.DataAtualizacao = DateTime.UtcNow;
        await _usuarioService.AtualizarAsync(usuario);

        var plainCodes = await GerarNovosRecoveryCodesAsync(usuario.Id);

        await _unitOfWork.CommitAsync();
        return new RecoveryCodesResponse { RecoveryCodes = plainCodes };
    }

    private async Task<IReadOnlyList<string>> GerarNovosRecoveryCodesAsync(Guid usuarioId)
    {
        await _recoveryCodeService.RemoverPorUsuarioAsync(usuarioId);

        var plainCodes = _mfaService.GenerateRecoveryCodes();
        var entidades = plainCodes.Select(code => new MfaRecoveryCode
        {
            UsuarioId = usuarioId,
            CodeHash = _mfaService.HashRecoveryCode(code)
        });

        await _recoveryCodeService.AdicionarRangeAsync(entidades);
        return plainCodes;
    }
}

// ============ Desabilitar ============
public class DisableMfaHandler : IRequestHandler<DisableMfaCommand, bool>
{
    private readonly IUsuarioService _usuarioService;
    private readonly IMfaService _mfaService;
    private readonly IMfaRecoveryCodeService _recoveryCodeService;
    private readonly IUnitOfWork _unitOfWork;

    public DisableMfaHandler(IUsuarioService usuarioService, IMfaService mfaService,
        IMfaRecoveryCodeService recoveryCodeService, IUnitOfWork unitOfWork)
    {
        _usuarioService = usuarioService;
        _mfaService = mfaService;
        _recoveryCodeService = recoveryCodeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DisableMfaCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioService.ObterPorIdAsync(request.UsuarioId)
            ?? throw new KeyNotFoundException("Usuário não encontrado");

        if (!usuario.MfaEnabled)
            throw new InvalidOperationException("MFA não está habilitado");

        var secret = _mfaService.Unprotect(usuario.MfaSecret!);
        if (!_mfaService.ValidateCode(secret, request.Code))
            throw new ValidationException("Código inválido");

        await LimparMfaAsync(usuario);
        await _unitOfWork.CommitAsync();
        return true;
    }

    private async Task LimparMfaAsync(Usuario usuario)
    {
        usuario.MfaEnabled = false;
        usuario.MfaSecret = null;
        usuario.MfaEnrolledAt = null;
        usuario.DataAtualizacao = DateTime.UtcNow;
        await _usuarioService.AtualizarAsync(usuario);
        await _recoveryCodeService.RemoverPorUsuarioAsync(usuario.Id);
    }
}

// ============ Regerar códigos de recuperação ============
public class RegenerateRecoveryCodesHandler : IRequestHandler<RegenerateRecoveryCodesCommand, RecoveryCodesResponse>
{
    private readonly IUsuarioService _usuarioService;
    private readonly IMfaService _mfaService;
    private readonly IMfaRecoveryCodeService _recoveryCodeService;
    private readonly IUnitOfWork _unitOfWork;

    public RegenerateRecoveryCodesHandler(IUsuarioService usuarioService, IMfaService mfaService,
        IMfaRecoveryCodeService recoveryCodeService, IUnitOfWork unitOfWork)
    {
        _usuarioService = usuarioService;
        _mfaService = mfaService;
        _recoveryCodeService = recoveryCodeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<RecoveryCodesResponse> Handle(RegenerateRecoveryCodesCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioService.ObterPorIdAsync(request.UsuarioId)
            ?? throw new KeyNotFoundException("Usuário não encontrado");

        if (!usuario.MfaEnabled || string.IsNullOrEmpty(usuario.MfaSecret))
            throw new InvalidOperationException("MFA não está habilitado");

        var secret = _mfaService.Unprotect(usuario.MfaSecret);
        if (!_mfaService.ValidateCode(secret, request.Code))
            throw new ValidationException("Código inválido");

        await _recoveryCodeService.RemoverPorUsuarioAsync(usuario.Id);

        var plainCodes = _mfaService.GenerateRecoveryCodes();
        var entidades = plainCodes.Select(code => new MfaRecoveryCode
        {
            UsuarioId = usuario.Id,
            CodeHash = _mfaService.HashRecoveryCode(code)
        });
        await _recoveryCodeService.AdicionarRangeAsync(entidades);

        await _unitOfWork.CommitAsync();
        return new RecoveryCodesResponse { RecoveryCodes = plainCodes };
    }
}

// ============ Reset administrativo ============
public class ResetMfaHandler : IRequestHandler<ResetMfaCommand, bool>
{
    private readonly IUsuarioService _usuarioService;
    private readonly IMfaRecoveryCodeService _recoveryCodeService;
    private readonly IUnitOfWork _unitOfWork;

    public ResetMfaHandler(IUsuarioService usuarioService, IMfaRecoveryCodeService recoveryCodeService, IUnitOfWork unitOfWork)
    {
        _usuarioService = usuarioService;
        _recoveryCodeService = recoveryCodeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ResetMfaCommand request, CancellationToken cancellationToken)
    {
        // Somente o usuário "admin" pode resetar o MFA de outros (mesmo padrão ad-hoc do restante da app).
        if (!string.Equals(request.SolicitanteLogin, "admin", StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Apenas o administrador pode resetar o MFA de outro usuário");

        var usuario = await _usuarioService.ObterPorIdAsync(request.UsuarioId)
            ?? throw new KeyNotFoundException("Usuário não encontrado");

        usuario.MfaEnabled = false;
        usuario.MfaSecret = null;
        usuario.MfaEnrolledAt = null;
        usuario.DataAtualizacao = DateTime.UtcNow;
        await _usuarioService.AtualizarAsync(usuario);
        await _recoveryCodeService.RemoverPorUsuarioAsync(usuario.Id);

        await _unitOfWork.CommitAsync();
        return true;
    }
}

// ============ Status (para a tela de configurações) ============
public class MfaStatusHandler : IRequestHandler<MfaStatusQuery, MfaStatusResponse>
{
    private readonly IUsuarioService _usuarioService;
    private readonly IMfaRecoveryCodeService _recoveryCodeService;

    public MfaStatusHandler(IUsuarioService usuarioService, IMfaRecoveryCodeService recoveryCodeService)
    {
        _usuarioService = usuarioService;
        _recoveryCodeService = recoveryCodeService;
    }

    public async Task<MfaStatusResponse> Handle(MfaStatusQuery request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioService.ObterPorIdAsync(request.UsuarioId)
            ?? throw new KeyNotFoundException("Usuário não encontrado");

        var restantes = usuario.MfaEnabled
            ? (await _recoveryCodeService.ObterAtivosPorUsuarioAsync(usuario.Id)).Count
            : 0;

        return new MfaStatusResponse { MfaEnabled = usuario.MfaEnabled, RecoveryCodesRestantes = restantes };
    }
}
