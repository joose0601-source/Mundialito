
using Microsoft.AspNetCore.Mvc;
using Mundialito.Application.Features.Jugadores.Commands;
using Mundialito.Application.Features.Jugadores.Queries;
using Mundialito.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mundialito.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JugadoresController : ControllerBase
{
    private readonly CrearJugadorCommandHandler _crearHandler;
    private readonly ActualizarJugadorCommandHandler _actualizarHandler;
    private readonly EliminarJugadorCommandHandler _eliminarHandler;
    private readonly ObtenerJugadoresQueryHandler _obtenerHandler;
    private readonly ObtenerJugadorPorIdQueryHandler _obtenerPorIdHandler;

    private readonly IIdempotencyService _idempotencyService;

    public JugadoresController(
        CrearJugadorCommandHandler crearHandler,
        ActualizarJugadorCommandHandler actualizarHandler,
        EliminarJugadorCommandHandler eliminarHandler,
        ObtenerJugadoresQueryHandler obtenerHandler,
        ObtenerJugadorPorIdQueryHandler obtenerPorIdHandler,
        IIdempotencyService idempotencyService)
    {
        _crearHandler = crearHandler;
        _actualizarHandler = actualizarHandler;
        _eliminarHandler = eliminarHandler;
        _obtenerHandler = obtenerHandler;
        _obtenerPorIdHandler = obtenerPorIdHandler;
        _idempotencyService = idempotencyService;
    }

    // POST: api/jugadores

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] CrearJugadorCommand command,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            return BadRequest(new
            {
                mensaje = "Debes enviar el encabezado Idempotency-Key.",
                codigo = "IDEMPOTENCY_KEY_REQUERIDA"
            });
        }

        var resultado = await _idempotencyService.CrearJugadorAsync(
            idempotencyKey,
            command,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(resultado.Location))
        {
            Response.Headers.Location = resultado.Location;
        }

        return new ContentResult
        {
            StatusCode = resultado.StatusCode,
            ContentType = "application/json; charset=utf-8",
            Content = resultado.ResponseBody
        };
    }

    // GET: api/jugadores
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? equipoId = null,
        [FromQuery] string? nombre = null,
        [FromQuery] string? posicion = null,
        [FromQuery] string sortBy = "nombre",
        [FromQuery] string sortDirection = "asc",
        CancellationToken cancellationToken = default)
    {
        var resultado = await _obtenerHandler.Handle(
            pageNumber,
            pageSize,
            equipoId,
            nombre,
            posicion,
            sortBy,
            sortDirection,
            cancellationToken);

        if (!resultado.IsSuccess)
        {
            return BadRequest(new
            {
                mensaje = resultado.ErrorMessage
            });
        }

        return Ok(resultado.Value);
    }


    // GET: api/jugadores/{id}
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

    // PUT: api/jugadores/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Actualizar(
        Guid id,
        [FromBody] ActualizarJugadorCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest(new
            {
                mensaje = "El ID de la ruta no coincide con el ID del jugador."
            });
        }

        var resultado = await _actualizarHandler.Handle(
            command, cancellationToken);

        if (!resultado.IsSuccess)
            return BadRequest(new { mensaje = resultado.ErrorMessage });

        return NoContent();
    }

    // DELETE: api/jugadores/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(
        Guid id,
        CancellationToken cancellationToken)
    {
        var resultado = await _eliminarHandler.Handle(
            new EliminarJugadorCommand(id), cancellationToken);

        if (!resultado.IsSuccess)
            return NotFound(new { mensaje = resultado.ErrorMessage });

        return NoContent();
    }
}
