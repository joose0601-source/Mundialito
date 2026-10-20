
using Mundialito.Application.Common;
using Mundialito.Application.Interfaces;
using Mundialito.Domain;

namespace Mundialito.Application.Features.Goles;

public sealed class RegistrarGolesCommandHandler
{
    private readonly IPartidoRepository _partidoRepository;
    private readonly IJugadorRepository _jugadorRepository;
    private readonly IGolPartidoRepository _golPartidoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarGolesCommandHandler(
        IPartidoRepository partidoRepository,
        IJugadorRepository jugadorRepository,
        IGolPartidoRepository golPartidoRepository,
        IUnitOfWork unitOfWork)
    {
        _partidoRepository = partidoRepository;
        _jugadorRepository = jugadorRepository;
        _golPartidoRepository = golPartidoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RegistrarGolesCommand command,
        CancellationToken cancellationToken = default,
        bool manageTransaction = true)
    {
        if (command.CantidadGoles <= 0)
        {
            return Result.Failure(
                "La cantidad de goles debe ser mayor que cero.",
                "GOLES_INVALIDOS");
        }

        var partido = await _partidoRepository.ObtenerPorIdAsync(
            command.PartidoId,
            cancellationToken);

        if (partido is null)
        {
            return Result.Failure(
                "El partido no existe.",
                "PARTIDO_NO_ENCONTRADO");
        }

        if (partido.Estado != EstadoPartido.Finalizado)
        {
            return Result.Failure(
                "Solo se pueden registrar goleadores en partidos finalizados.",
                "PARTIDO_NO_FINALIZADO");
        }

        var jugador = await _jugadorRepository.ObtenerPorIdAsync(
            command.JugadorId,
            cancellationToken);

        if (jugador is null)
        {
            return Result.Failure(
                "El jugador no existe.",
                "JUGADOR_NO_ENCONTRADO");
        }

        var perteneceAlPartido =
            jugador.EquipoId == partido.EquipoLocalId ||
            jugador.EquipoId == partido.EquipoVisitanteId;

        if (!perteneceAlPartido)
        {
            return Result.Failure(
                "El jugador no pertenece a ninguno de los equipos del partido.",
                "JUGADOR_NO_PARTICIPANTE");
        }

        var equipoId = jugador.EquipoId;

        var golesMarcador = equipoId == partido.EquipoLocalId
            ? partido.GolesLocal!.Value
            : partido.GolesVisitante!.Value;

        var golesRegistrados =
            await _golPartidoRepository.ObtenerGolesRegistradosPorEquipoAsync(
                partido.Id,
                equipoId,
                cancellationToken);

        if (golesRegistrados + command.CantidadGoles > golesMarcador)
        {
            return Result.Failure(
                "La cantidad de goles individuales superaría el marcador oficial del equipo.",
                "GOLES_SUPERAN_MARCADOR");
        }

        var golPartido = new GolPartido(
            partido.Id,
            jugador.Id,
            command.CantidadGoles);

        await _golPartidoRepository.AgregarAsync(
            golPartido,
            cancellationToken);

        if (manageTransaction)
        {
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        return Result.Success();
    }
}
