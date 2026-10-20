
using Microsoft.AspNetCore.Mvc;
using Mundialito.Application.Interfaces;

namespace Mundialito.Api.Controllers;

[ApiController]
[Route("api/goleadores")]
public sealed class GoleadoresController : ControllerBase
{
    private readonly IGoleadoresQueryRepository _repository;

    public GoleadoresController(IGoleadoresQueryRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTabla(
        CancellationToken cancellationToken)
    {
        var goleadores = await _repository.ObtenerTablaAsync(
            cancellationToken);

        return Ok(goleadores);
    }
}
