using CadastroVeiculo.Domain.Enums;
using CadastroVeiculos.Application.Features.Veiculos.Commands;
using CadastroVeiculos.Application.Features.Veiculos.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadastroVeiculos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VeiculosController : ControllerBase
{
    private readonly IMediator _mediator;

    public VeiculosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AdicionarVeiculoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AdicionarVeiculoResponse>> Cadastrar([FromBody] CadastrarVeiculoRequest request)
    {
        try
        {
            var command = new AdicionarVeiculoCommand
            {
                Descricao = request.Descricao,
                Marca = (Marca)request.Marca,
                Modelo = request.Modelo,
                Opcionais = request.Opcionais,
                Valor = request.Valor
            };

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ObterVeiculoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ObterVeiculoResponse>> ObterPorId(Guid id)
    {
        var query = new ObterVeiculoPorIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ListarVeiculoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ListarVeiculoResponse>>> Listar()
    {
        var query = new ListarVeiculosQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AtualizarVeiculoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AtualizarVeiculoResponse>> Atualizar(Guid id, [FromBody] AtualizarVeiculoRequest request)
    {
        try
        {
            var command = new AtualizarVeiculoCommand
            {
                Id = id,
                Descricao = request.Descricao,
                Marca = (Marca)request.Marca,
                Modelo = request.Modelo,
                Opcionais = request.Opcionais,
                Valor = request.Valor
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Excluir(Guid id)
    {
        try
        {
            var command = new ExcluirVeiculoCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

public class CadastrarVeiculoRequest
{
    public string Descricao { get; set; } = string.Empty;
    public int Marca { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public string? Opcionais { get; set; }
    public decimal? Valor { get; set; }
}

public class AtualizarVeiculoRequest
{
    public string Descricao { get; set; } = string.Empty;
    public int Marca { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public string? Opcionais { get; set; }
    public decimal? Valor { get; set; }
}