
using Microsoft.EntityFrameworkCore;
using Mundialito.Application.Interfaces;
using Mundialito.Domain;
using Mundialito.Infrastructure.Persistence;

namespace Mundialito.Infrastructure.Repositories;

public sealed class EquipoRepository : IEquipoRepository
{
    private readonly ApplicationDbContext _context;

    public EquipoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AgregarAsync(
        Equipo equipo,
        CancellationToken cancellationToken = default)
    {
        await _context.Equipos.AddAsync(equipo, cancellationToken);
    }

    public async Task<Equipo?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Equipos
            .FirstOrDefaultAsync(
                equipo => equipo.Id == id,
                cancellationToken);
    }

    public void Eliminar(Equipo equipo)
    {
        _context.Equipos.Remove(equipo);
    }
}
