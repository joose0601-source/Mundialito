
using Microsoft.EntityFrameworkCore;
using Mundialito.Application.Interfaces;
using Mundialito.Domain;
using Mundialito.Infrastructure.Persistence;

namespace Mundialito.Infrastructure.Repositories;

public sealed class PartidoRepository : IPartidoRepository
{
    private readonly ApplicationDbContext _context;

    public PartidoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AgregarAsync(
        Partido partido,
        CancellationToken cancellationToken = default)
    {
        await _context.Partidos.AddAsync(partido, cancellationToken);
    }

    public async Task<Partido?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Partidos
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> ExisteEquipoAsync(
        Guid equipoId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Equipos
            .AnyAsync(e => e.Id == equipoId, cancellationToken);
    }

    public void Actualizar(Partido partido)
    {
        _context.Partidos.Update(partido);
    }

    public void Eliminar(Partido partido)
    {
        _context.Partidos.Remove(partido);
    }
}
