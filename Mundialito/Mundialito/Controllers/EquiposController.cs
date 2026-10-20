
using Microsoft.AspNetCore.Mvc;
using Mundialito.Application.Features.Equipos.Commands;
using Mundialito.Application.Features.Equipos.Queries;
using Mundialito.Application.Interfaces;

namespace Mundialito.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EquiposController : ControllerBase
{
    private readonly CrearEquipoCommandHandler _crearEquipoHandler;
    private readonly ObtenerEquiposQueryHandler _obtenerEquiposHandler;
    private readonly ObtenerEquipoPorIdQueryHandler _obtenerEquipoPorIdHandler;

    private readonly ActualizarEquipoCommandHandler _actualizarEquipoHandler;

    private readonly EliminarEquipoCommandHandler _eliminarEquipoHandler;

    private readonly IIdempotencyService _idempotencyService;
    public EquiposController(
        CrearEquipoCommandHandler crearEquipoHandler,
        ObtenerEquiposQueryHandler obtenerEquiposHandler,
        ObtenerEquipoPorIdQueryHandler obtenerEquipoPorIdHandler,
            ActualizarEquipoCommandHandler actualizarEquipoHandler,
            EliminarEquipoCommandHandler eliminarEquipoHandler,
            IIdempotencyService idempotencyService)
    {
        _crearEquipoHandler = crearEquipoHandler;
        _obtenerEquiposHandler = obtenerEquiposHandler;
        _obtenerEquipoPorIdHandler = obtenerEquipoPorIdHandler;
        _actualizarEquipoHandler = actualizarEquipoHandler;
        _eliminarEquipoHandler = eliminarEquipoHandler;
        _idempotencyService = idempotencyService;

    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] CrearEquipoCommand command,
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

        var resultado = await _idempotencyService.CrearEquipoAsync(
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



    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? nombre = null,
        [FromQuery] string? entrenador = null,
        [FromQuery] string sortBy = "nombre",
        [FromQuery] string sortDirection = "asc",
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            return BadRequest(new
            {
                mensaje = "La página debe ser mayor que cero."
            });
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new
            {
                mensaje = "El tamaño de página debe estar entre 1 y 100."
            });
        }

        var columnasPermitidas = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
    {
        "nombre",
        "entrenador",
        "fechaCreacion"
    };

        if (!columnasPermitidas.Contains(sortBy))
        {
            return BadRequest(new
            {
                mensaje = "El campo de ordenamiento no es válido."
            });
        }

        if (!sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase)
            && !sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                mensaje = "La dirección debe ser 'asc' o 'desc'."
            });
        }

        var resultado = await _obtenerEquiposHandler.HandleAsync(
            page,
            pageSize,
            nombre,
            entrenador,
            sortBy,
            sortDirection,
            cancellationToken);

        return Ok(resultado);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(
        Guid id,
        CancellationToken cancellationToken)
    {
        var equipo = await _obtenerEquipoPorIdHandler.HandleAsync(
            id,
            cancellationToken);

        if (equipo is null)
        {
            return NotFound(new
            {
                mensaje = "No se encontró el equipo solicitado."
            });
        }

        return Ok(equipo);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Actualizar(
        Guid id,
        [FromBody] ActualizarEquipoCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest(new
            {
                mensaje = "El ID de la URL no coincide con el ID del equipo enviado."
            });
        }

        var resultado = await _actualizarEquipoHandler.HandleAsync(
            command,
            cancellationToken);

        if (resultado.IsFailure)
        {
            if (resultado.ErrorCode == "EQUIPO_NO_ENCONTRADO")
            {
                return NotFound(new
                {
                    mensaje = resultado.ErrorMessage,
                    codigo = resultado.ErrorCode
                });
            }

            return BadRequest(new
            {
                mensaje = resultado.ErrorMessage,
                codigo = resultado.ErrorCode
            });
        }

        return Ok(new
        {
            id = resultado.Value,
            mensaje = "Equipo actualizado correctamente"
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(
        Guid id,
        CancellationToken cancellationToken)
    {
        var resultado = await _eliminarEquipoHandler.HandleAsync(
            new EliminarEquipoCommand(id),
            cancellationToken);

        if (resultado.IsFailure)
        {
            if (resultado.ErrorCode == "EQUIPO_NO_ENCONTRADO")
            {
                return NotFound(new
                {
                    mensaje = resultado.ErrorMessage,
                    codigo = resultado.ErrorCode
                });
            }

            return BadRequest(new
            {
                mensaje = resultado.ErrorMessage,
                codigo = resultado.ErrorCode
            });
        }

        return NoContent();
    }


}
