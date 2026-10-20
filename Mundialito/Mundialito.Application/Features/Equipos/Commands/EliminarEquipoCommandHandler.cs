
using Mundialito.Application.Common;
using Mundialito.Application.Interfaces;

namespace Mundialito.Application.Features.Equipos.Commands;

public sealed class EliminarEquipoCommandHandler
{
    private readonly IEquipoRepository _equipoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarEquipoCommandHandler(
        IEquipoRepository equipoRepository,
        IUnitOfWork unitOfWork)
    {
        _equipoRepository = equipoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(
        EliminarEquipoCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.Id == Guid.Empty)
        {
            return Result.Failure(
                "El identificador del equipo no es válido.",
                "ID_INVALIDO");
        }

        var equipo = await _equipoRepository.ObtenerPorIdAsync(
            command.Id,
            cancellationToken);

        if (equipo is null)
        {
            return Result.Failure(
                "No se encontró el equipo solicitado.",
                "EQUIPO_NO_ENCONTRADO");
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            _equipoRepository.Eliminar(equipo);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(
                CancellationToken.None);

            throw;
        }
    }
}
