
using Mundialito.Domain;

namespace Mundialito.Application.Interfaces;

public interface IEquipoRepository
{
    Task AgregarAsync(
        Equipo equipo,
        CancellationToken cancellationToken = default);

    Task<Equipo?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    void Eliminar(Equipo equipo);
}
