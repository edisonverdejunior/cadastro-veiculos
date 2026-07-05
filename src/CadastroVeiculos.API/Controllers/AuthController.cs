using System.Security.Claims;
using CadastroVeiculos.Application.Features.Auth.AuthQueries;
using CadastroVeiculos.Application.Features.Mfa.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadastroVeiculos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var query = new LoginQuery { Login = request.Login, Senha = request.Senha };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "Login ou senha inválidos" });
        }
    }

    /// <summary>Etapa 2 do login: valida o código TOTP (ou de recuperação) e emite o token final.</summary>
    [HttpPost("login/mfa")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> LoginMfa([FromBody] MfaLoginRequest request)
    {
        try
        {
            var command = new MfaLoginCommand { MfaToken = request.MfaToken, Code = request.Code };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>Inicia o cadastro do MFA: gera segredo e URI otpauth:// para o QR Code.</summary>
    [HttpPost("mfa/setup")]
    [Authorize]
    [ProducesResponseType(typeof(SetupMfaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SetupMfaResponse>> SetupMfa()
    {
        try
        {
            var result = await _mediator.Send(new SetupMfaCommand { UsuarioId = GetUserId() });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Ativa o MFA validando o primeiro código e devolve os códigos de recuperação (uma única vez).</summary>
    [HttpPost("mfa/enable")]
    [Authorize]
    [ProducesResponseType(typeof(RecoveryCodesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecoveryCodesResponse>> EnableMfa([FromBody] MfaCodeRequest request)
    {
        try
        {
            var command = new EnableMfaCommand { UsuarioId = GetUserId(), Code = request.Code };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Desabilita o MFA (exige um código TOTP válido).</summary>
    [HttpPost("mfa/disable")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DisableMfa([FromBody] MfaCodeRequest request)
    {
        try
        {
            await _mediator.Send(new DisableMfaCommand { UsuarioId = GetUserId(), Code = request.Code });
            return NoContent();
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Gera um novo conjunto de códigos de recuperação (exige um código TOTP válido).</summary>
    [HttpPost("mfa/recovery-codes/regenerate")]
    [Authorize]
    [ProducesResponseType(typeof(RecoveryCodesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecoveryCodesResponse>> RegenerateRecoveryCodes([FromBody] MfaCodeRequest request)
    {
        try
        {
            var command = new RegenerateRecoveryCodesCommand { UsuarioId = GetUserId(), Code = request.Code };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Status do MFA do usuário logado (para a tela de configurações).</summary>
    [HttpGet("mfa/status")]
    [Authorize]
    [ProducesResponseType(typeof(MfaStatusResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<MfaStatusResponse>> MfaStatus()
    {
        var result = await _mediator.Send(new MfaStatusQuery { UsuarioId = GetUserId() });
        return Ok(result);
    }

    private Guid GetUserId()
    {
        var sub = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(sub, out var id) ? id : throw new UnauthorizedAccessException();
    }
}

public class LoginRequest
{
    public string Login { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class MfaLoginRequest
{
    public string MfaToken { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class MfaCodeRequest
{
    public string Code { get; set; } = string.Empty;
}
