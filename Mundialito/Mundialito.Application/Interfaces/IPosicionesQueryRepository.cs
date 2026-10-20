
using Mundialito.Application.Features.Posiciones.Queries;

namespace Mundialito.Application.Interfaces;

public interface IPosicionesQueryRepository
{
    Task<IReadOnlyList<PosicionEquipoDto>> ObtenerTablaAsync(
        CancellationToken cancellationToken = default);
}
