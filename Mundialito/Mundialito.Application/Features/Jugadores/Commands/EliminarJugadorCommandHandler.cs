
using Mundialito.Application.Common;
using Mundialito.Application.Interfaces;

namespace Mundialito.Application.Features.Jugadores.Commands;

public sealed class EliminarJugadorCommandHandler
{
    private readonly IJugadorRepository _jugadorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarJugadorCommandHandler(
        IJugadorRepository jugadorRepository,
        IUnitOfWork unitOfWork)
    {
        _jugadorRepository = jugadorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        EliminarJugadorCommand command,
        CancellationToken cancellationToken = default)
    {
        var jugador = await _jugadorRepository.ObtenerPorIdAsync(
            command.Id,
            cancellationToken);

        if (jugador is null)
            return Result<bool>.Failure("El jugador no existe.");

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            _jugadorRepository.Eliminar(jugador);

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
