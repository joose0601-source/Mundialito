
using Microsoft.EntityFrameworkCore;
using Mundialito.Application.Interfaces;
using Mundialito.Domain;
using Mundialito.Infrastructure.Persistence;

namespace Mundialito.Infrastructure.Repositories;

public sealed class JugadorRepository : IJugadorRepository
{
    private readonly ApplicationDbContext _context;

    public JugadorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AgregarAsync(
        Jugador jugador,
        CancellationToken cancellationToken = default)
    {
        await _context.Jugadores.AddAsync(jugador, cancellationToken);
    }

    public async Task<Jugador?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Jugadores
            .FirstOrDefaultAsync(
                jugador => jugador.Id == id,
                cancellationToken);
    }

    public void Eliminar(Jugador jugador)
    {
        _context.Jugadores.Remove(jugador);
    }
}
