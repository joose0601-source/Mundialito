
using Mundialito.Application.Common;
using Mundialito.Application.Interfaces;

namespace Mundialito.Application.Features.Partidos.Queries;

public sealed class ObtenerPartidoPorIdQueryHandler
{
    private readonly IPartidoQueryRepository _repository;

    public ObtenerPartidoPorIdQueryHandler(
        IPartidoQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PartidoDto>> Handle(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<PartidoDto>.Failure(
                "El identificador del partido no es válido.");
        }

        var partido = await _repository.ObtenerPorIdAsync(
            id,
            cancellationToken);

        if (partido is null)
        {
            return Result<PartidoDto>.Failure(
                "El partido no existe.",
                "PARTIDO_NO_ENCONTRADO");
        }

        return Result<PartidoDto>.Success(partido);
    }
}
