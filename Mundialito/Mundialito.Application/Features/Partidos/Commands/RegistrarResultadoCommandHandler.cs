
using Mundialito.Application.Common;
using Mundialito.Application.Interfaces;

namespace Mundialito.Application.Features.Partidos.Commands;

public sealed class RegistrarResultadoCommandHandler
{
    private readonly IPartidoRepository _partidoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarResultadoCommandHandler(
        IPartidoRepository partidoRepository,
        IUnitOfWork unitOfWork)
    {
        _partidoRepository = partidoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RegistrarResultadoCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.GolesLocal < 0 || command.GolesVisitante < 0)
        {
            return Result.Failure(
                "Los goles no pueden ser negativos.",
                "GOLES_NEGATIVOS");
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

        try
        {
            partido.RegistrarResultado(
                command.GolesLocal,
                command.GolesVisitante);

            _partidoRepository.Actualizar(partido);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(
                ex.Message,
                "RESULTADO_INVALIDO");
        }
        catch (InvalidOperationException ex)
        {
            var codigo = partido.Estado switch
            {
                Mundialito.Domain.EstadoPartido.Suspendido =>
                    "PARTIDO_SUSPENDIDO",

                Mundialito.Domain.EstadoPartido.Finalizado =>
                    "PARTIDO_YA_FINALIZADO",

                _ => "RESULTADO_INVALIDO"
            };

            return Result.Failure(ex.Message, codigo);
        }
    }
}
