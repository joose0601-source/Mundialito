
using Mundialito.Application.Common;
using Mundialito.Application.Interfaces;

namespace Mundialito.Application.Features.Jugadores.Commands;

public sealed class ActualizarJugadorCommandHandler
{
    private readonly IJugadorRepository _jugadorRepository;
    private readonly IJugadorQueryRepository _jugadorQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarJugadorCommandHandler(
        IJugadorRepository jugadorRepository,
        IJugadorQueryRepository jugadorQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _jugadorRepository = jugadorRepository;
        _jugadorQueryRepository = jugadorQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        ActualizarJugadorCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Nombre))
            return Result<bool>.Failure(
                "El nombre del jugador es obligatorio.");

        if (command.NumeroCamiseta < 1 || command.NumeroCamiseta > 99)
            return Result<bool>.Failure(
                "El número de camiseta debe estar entre 1 y 99.");

        if (string.IsNullOrWhiteSpace(command.Posicion))
            return Result<bool>.Failure(
                "La posición del jugador es obligatoria.");

        var jugador = await _jugadorRepository.ObtenerPorIdAsync(
            command.Id, cancellationToken);

        if (jugador is null)
            return Result<bool>.Failure("El jugador no existe.");

        var numeroOcupado =
            await _jugadorQueryRepository.ExisteNumeroCamisetaAsync(
                jugador.EquipoId,
                command.NumeroCamiseta,
                command.Id,
                cancellationToken);

        if (numeroOcupado)
            return Result<bool>.Failure(
                "Ese número de camiseta ya está asignado en este equipo.");

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            jugador.Actualizar(
                command.Nombre,
                command.NumeroCamiseta,
                command.Posicion);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
