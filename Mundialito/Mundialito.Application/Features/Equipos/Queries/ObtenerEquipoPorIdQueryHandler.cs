
using Mundialito.Application.Interfaces;

namespace Mundialito.Application.Features.Equipos.Queries;

public sealed class ObtenerEquipoPorIdQueryHandler
{
    private readonly IEquipoQueryRepository _repository;

    public ObtenerEquipoPorIdQueryHandler(
        IEquipoQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<EquipoDto?> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return null;
        }

        return await _repository.ObtenerPorIdAsync(
            id,
            cancellationToken);
    }
}
