
using Mundialito.Application.Common;
using Mundialito.Application.Interfaces;
using Mundialito.Domain;

namespace Mundialito.Application.Features.Partidos.Commands;

public sealed class CrearPartidoCommandHandler
{
    private readonly IPartidoRepository _partidoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearPartidoCommandHandler(
        IPartidoRepository partidoRepository,
        IUnitOfWork unitOfWork)
    {
        _partidoRepository = partidoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CrearPartidoCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.EquipoLocalId == command.EquipoVisitanteId)
        {
            return Result<Guid>.Failure(
                "El equipo local y el visitante deben ser diferentes.");
        }

        var localExiste = await _partidoRepository.ExisteEquipoAsync(
            command.EquipoLocalId, cancellationToken);

        if (!localExiste)
        {
            return Result<Guid>.Failure(
                "El equipo local no existe.");
        }

        var visitanteExiste = await _partidoRepository.ExisteEquipoAsync(
            command.EquipoVisitanteId, cancellationToken);

        if (!visitanteExiste)
        {
            return Result<Guid>.Failure(
                "El equipo visitante no existe.");
        }

        try
        {
            var partido = new Partido(
                command.EquipoLocalId,
                command.EquipoVisitanteId,
                command.FechaPartido);

            await _partidoRepository.AgregarAsync(
                partido, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid>.Success(partido.Id);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
    }
}
