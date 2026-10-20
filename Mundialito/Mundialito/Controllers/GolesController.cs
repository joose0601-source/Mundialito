
using Microsoft.AspNetCore.Mvc;
using Mundialito.Application.Features.Goles;
using Mundialito.Application.Interfaces;

namespace Mundialito.Api.Controllers;

[ApiController]
[Route("api/goles")]
public sealed class GolesController : ControllerBase
{
    private readonly IIdempotencyService _idempotencyService;

    public GolesController(IIdempotencyService idempotencyService)
    {
        _idempotencyService = idempotencyService;
    }

    [HttpPost]
    public async Task<IActionResult> Registrar(
        [FromBody] RegistrarGolesCommand command,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            return BadRequest(new
            {
                codigo = "IDEMPOTENCY_KEY_INVALIDA",
                mensaje = "Debes enviar el encabezado Idempotency-Key."
            });
        }

        var resultado = await _idempotencyService.RegistrarGolesAsync(
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
            Content = resultado.ResponseBody,
            ContentType = "application/json"
        };
    }
}
