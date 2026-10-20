
using Microsoft.AspNetCore.Mvc;
using Mundialito.Application.Features.Partidos.Commands;
using Mundialito.Application.Features.Partidos.Queries;

namespace Mundialito.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PartidosController : ControllerBase
{
    private readonly CrearPartidoCommandHandler _crearHandler;
    private readonly RegistrarResultadoCommandHandler _resultadoHandler;
    private readonly ObtenerPartidosQueryHandler _obtenerHandler;
    private readonly ObtenerPartidoPorIdQueryHandler _obtenerPorIdHandler;

    public PartidosController(
        CrearPartidoCommandHandler crearHandler,
        RegistrarResultadoCommandHandler resultadoHandler,
        ObtenerPartidosQueryHandler obtenerHandler,
        ObtenerPartidoPorIdQueryHandler obtenerPorIdHandler)
    {
        _crearHandler = crearHandler;
        _resultadoHandler = resultadoHandler;
        _obtenerHandler = obtenerHandler;
        _obtenerPorIdHandler = obtenerPorIdHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] CrearPartidoCommand command,
        CancellationToken cancellationToken)
    {
        var resultado = await _crearHandler.Handle(command, cancellationToken);

        if (!resultado.IsSuccess)
            return BadRequest(new { mensaje = resultado.ErrorMessage });

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = resultado.Value },
            new { id = resultado.Value });
    }


    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 10,
        [FromQuery] Guid? equipoId = null,
        [FromQuery] int? estado = null,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        [FromQuery] string sortBy = "fechaPartido",
        [FromQuery] string sortDirection = "asc",
        CancellationToken cancellationToken = default)
    {
        var resultado = await _obtenerHandler.Handle(
            pagina,
            tamanoPagina,
            equipoId,
            estado,
            fechaDesde,
            fechaHasta,
            sortBy,
            sortDirection,
            cancellationToken);

        if (!resultado.IsSuccess)
            return BadRequest(new { mensaje = resultado.ErrorMessage });

        return Ok(resultado.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(
        Guid id,
        CancellationToken cancellationToken)
    {
        var resultado = await _obtenerPorIdHandler.Handle(
            id, cancellationToken);

        if (!resultado.IsSuccess)
            return NotFound(new { mensaje = resultado.ErrorMessage });

        return Ok(resultado.Value);
    }

    [HttpPut("{id:guid}/resultado")]
    public async Task<IActionResult> RegistrarResultado(
        Guid id,
        [FromBody] RegistrarResultadoCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.PartidoId)
        {
            return BadRequest(new
            {
                mensaje = "El ID de la ruta no coincide con el ID del partido."
            });
        }

        var resultado = await _resultadoHandler.Handle(
            command, cancellationToken);
        if (!resultado.IsSuccess)
        {
            return MapearErrorResultado(
                resultado.ErrorCode,
                resultado.ErrorMessage);
        }

        return NoContent();
    }


    private IActionResult MapearErrorResultado(
    string? errorCode,
    string? errorMessage)
    {
        var mensaje = new { mensaje = errorMessage };

        return errorCode switch
        {
            "PARTIDO_NO_ENCONTRADO" =>
                NotFound(mensaje),

            "PARTIDO_YA_FINALIZADO" or "PARTIDO_SUSPENDIDO" =>
                Conflict(mensaje),

            "GOLES_NEGATIVOS" or "RESULTADO_INVALIDO" =>
                BadRequest(mensaje),

            _ => BadRequest(mensaje)
        };
    }
}
