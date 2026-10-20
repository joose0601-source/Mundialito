
using Mundialito.Application.Common;
using Mundialito.Application.Interfaces;
using Mundialito.Domain;

namespace Mundialito.Application.Features.Equipos.Commands;

public sealed class CrearEquipoCommandHandler
{
    private readonly IEquipoRepository _equipoRepository;
    private readonly IEquipoQueryRepository _equipoQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearEquipoCommandHandler(
        IEquipoRepository equipoRepository,
        IEquipoQueryRepository equipoQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _equipoRepository = equipoRepository;
        _equipoQueryRepository = equipoQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(
        CrearEquipoCommand command,
        CancellationToken cancellationToken = default,
        bool manageTransaction = true)
    {
        if (string.IsNullOrWhiteSpace(command.Nombre))
        {
            return Result<Guid>.Failure(
                "El nombre del equipo es obligatorio.",
                "NOMBRE_REQUERIDO");
        }

        if (string.IsNullOrWhiteSpace(command.Entrenador))
        {
            return Result<Guid>.Failure(
                "El nombre del entrenador es obligatorio.",
                "ENTRENADOR_REQUERIDO");
        }

        var nombre = command.Nombre.Trim();

        if (await _equipoQueryRepository.ExisteAsync(
            nombre, cancellationToken))
        {
            return Result<Guid>.Failure(
                "Ya existe un equipo con ese nombre.",
                "EQUIPO_DUPLICADO");
        }


        try
        {
            if (manageTransaction)
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);
            }

            var equipo = new Equipo(nombre, command.Entrenador);

            await _equipoRepository.AgregarAsync(
                equipo, cancellationToken);

            if (manageTransaction)
            {
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }

            return Result<Guid>.Success(equipo.Id);
        }
        catch (ArgumentException ex)
        {
            if (manageTransaction)
            {
                await _unitOfWork.RollbackTransactionAsync(
                    CancellationToken.None);
            }

            return Result<Guid>.Failure(
                ex.Message, "DATOS_INVALIDOS");
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
