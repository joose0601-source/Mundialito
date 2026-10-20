using Mundialito.Domain;

namespace Mundialito.Application.Interfaces;

public interface IGolPartidoRepository
{
    Task AgregarAsync(
        GolPartido golPartido,
        CancellationToken cancellationToken = default);

    Task<int> ObtenerGolesRegistradosPorEquipoAsync(
        Guid partidoId,
        Guid equipoId,
        CancellationToken cancellationToken = default);
}