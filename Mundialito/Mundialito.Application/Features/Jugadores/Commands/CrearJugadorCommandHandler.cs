
using Mundialito.Application.Common;
using Mundialito.Application.Interfaces;
using Mundialito.Domain;

namespace Mundialito.Application.Features.Jugadores.Commands;

public sealed class CrearJugadorCommandHandler
{
    private readonly IJugadorRepository _jugadorRepository;
    private readonly IJugadorQueryRepository _jugadorQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearJugadorCommandHandler(
        IJugadorRepository jugadorRepository,
        IJugadorQueryRepository jugadorQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _jugadorRepository = jugadorRepository;
        _jugadorQueryRepository = jugadorQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CrearJugadorCommand command,
        CancellationToken cancellationToken = default,
        bool manageTransaction = true)
    {
        if (string.IsNullOrWhiteSpace(command.Nombre))
            return Result<Guid>.Failure(
                "El nombre del jugador es obligatorio.");

        if (command.NumeroCamiseta < 1 || command.NumeroCamiseta > 99)
            return Result<Guid>.Failure(
                "El número de camiseta debe estar entre 1 y 99.");

        if (string.IsNullOrWhiteSpace(command.Posicion))
            return Result<Guid>.Failure(
                "La posición del jugador es obligatoria.");

        var existeEquipo = await _jugadorQueryRepository.ExisteEquipoAsync(
            command.EquipoId, cancellationToken);

        if (!existeEquipo)
            return Result<Guid>.Failure(
                "El equipo indicado no existe.");

        var numeroOcupado =
            await _jugadorQueryRepository.ExisteNumeroCamisetaAsync(
                command.EquipoId,
                command.NumeroCamiseta,
                cancellationToken: cancellationToken);

        if (numeroOcupado)
            return Result<Guid>.Failure(
                "Ese número de camiseta ya está asignado en este equipo.");

        try
        {
            if (manageTransaction)
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var jugador = new Jugador(
                command.Nombre.Trim(),
                command.NumeroCamiseta,
                command.Posicion.Trim(),
                command.EquipoId);

            await _jugadorRepository.AgregarAsync(
                jugador, cancellationToken);

            if (manageTransaction)
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<Guid>.Success(jugador.Id);
        }
        catch
        {
            if (manageTransaction)
            {
                await _unitOfWork.RollbackTransactionAsync(
                    CancellationToken.None);
            }

            throw;
        }
    }
}
