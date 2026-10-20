
using Mundialito.Domain;

namespace Mundialito.Application.Interfaces;

public interface IJugadorRepository
{
    Task AgregarAsync(Jugador jugador, CancellationToken cancellationToken = default);

    Task<Jugador?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Eliminar(Jugador jugador);
}
