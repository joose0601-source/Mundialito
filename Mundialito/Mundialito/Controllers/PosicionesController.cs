
using Microsoft.AspNetCore.Mvc;
using Mundialito.Application.Interfaces;

namespace Mundialito.Api.Controllers;

[ApiController]
[Route("api/posiciones")]
public sealed class PosicionesController : ControllerBase
{
    private readonly IPosicionesQueryRepository _posicionesRepository;

    public PosicionesController(
        IPosicionesQueryRepository posicionesRepository)
    {
        _posicionesRepository = posicionesRepository;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTabla(
        CancellationToken cancellationToken)
    {
        var posiciones = await _posicionesRepository.ObtenerTablaAsync(
            cancellationToken);

        return Ok(posiciones);
    }
}
