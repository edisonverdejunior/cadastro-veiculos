using CadastroVeiculos.Application.Features.Auth.AuthQueries;
using MediatR;
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
}

public class LoginRequest
{
    public string Login { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
