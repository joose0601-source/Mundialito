
using Mundialito.Application.Features.Goles.Queries;

namespace Mundialito.Application.Interfaces;

public interface IGoleadoresQueryRepository
{
    Task<IReadOnlyList<GoleadorDto>> ObtenerTablaAsync(
        CancellationToken cancellationToken = default);
}
