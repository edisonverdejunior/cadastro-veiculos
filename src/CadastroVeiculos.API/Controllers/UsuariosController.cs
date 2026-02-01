using CadastroVeiculos.Application.Features.Usuarios.Commands;
using CadastroVeiculos.Application.Features.Usuarios.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadastroVeiculos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsuariosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CadastrarUsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CadastrarUsuarioResponse>> Cadastrar([FromBody] CadastrarUsuarioRequest request)
    {
        try
        {
            var command = new CadastrarUsuarioCommand
            {
                Nome = request.Nome,
                Login = request.Login,
                Senha = request.Senha
            };

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (Exception ex) when (ex.Message.Contains("Login já existe"))
        {
            return BadRequest(new { message = "Login já existe" });
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ObterUsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ObterUsuarioResponse>> ObterPorId(Guid id)
    {
        var query = new ObterUsuarioPorIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<ListarUsuarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ListarUsuarioResponse>>> Listar()
    {
        var query = new ListarUsuariosQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(AtualizarUsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AtualizarUsuarioResponse>> Atualizar(Guid id, [FromBody] AtualizarUsuarioRequest request)
    {
        try
        {
            var command = new AtualizarUsuarioCommand
            {
                Id = id,
                Nome = request.Nome
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Excluir(Guid id)
    {
        try
        {
            var command = new ExcluirUsuarioCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

public class CadastrarUsuarioRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class AtualizarUsuarioRequest
{
    public string Nome { get; set; } = string.Empty;
}