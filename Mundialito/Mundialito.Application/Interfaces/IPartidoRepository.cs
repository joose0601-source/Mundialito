
using Mundialito.Domain;

namespace Mundialito.Application.Interfaces;

public interface IPartidoRepository
{
    Task AgregarAsync(
        Partido partido,
        CancellationToken cancellationToken = default);

    Task<Partido?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteEquipoAsync(
        Guid equipoId,
        CancellationToken cancellationToken = default);

    void Actualizar(Partido partido);

    void Eliminar(Partido partido);
}
