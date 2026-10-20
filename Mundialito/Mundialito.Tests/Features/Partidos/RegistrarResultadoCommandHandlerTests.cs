
using Moq;
using Mundialito.Application.Common;
using Mundialito.Application.Features.Partidos.Commands;
using Mundialito.Application.Interfaces;
using Mundialito.Domain;
using Xunit;

namespace Mundialito.Tests.Features.Partidos;

public sealed class RegistrarResultadoCommandHandlerTests
{
    private readonly Mock<IPartidoRepository> _partidoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RegistrarResultadoCommandHandler _handler;

    public RegistrarResultadoCommandHandlerTests()
    {
        _partidoRepositoryMock = new Mock<IPartidoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new RegistrarResultadoCommandHandler(
            _partidoRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ResultadoValido_DebeGuardarCambios()
    {
        // Arrange
        var partido = CrearPartido();

        var command = new RegistrarResultadoCommand(
            partido.Id,
            3,
            1);

        _partidoRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(
                partido.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(partido);

        _unitOfWorkMock
            .Setup(u => u.CommitAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        Result resultado = await _handler.Handle(command);

        // Assert
        Assert.True(resultado.IsSuccess);
        Assert.Equal(3, partido.GolesLocal);
        Assert.Equal(1, partido.GolesVisitante);
        Assert.Equal(EstadoPartido.Finalizado, partido.Estado);

        _partidoRepositoryMock.Verify(
            r => r.Actualizar(partido),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_GolesNegativos_DebeRechazarResultado()
    {
        // Arrange
        var partido = CrearPartido();

        var command = new RegistrarResultadoCommand(
            partido.Id,
            -1,
            2);

        // Act
        Result resultado = await _handler.Handle(command);

        // Assert
        Assert.True(resultado.IsFailure);
        Assert.Equal(
            "Los goles no pueden ser negativos.",
            resultado.ErrorMessage);

        _partidoRepositoryMock.Verify(
            r => r.ObtenerPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            u => u.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_PartidoInexistente_DebeDevolverError()
    {
        // Arrange
        var partidoId = Guid.NewGuid();

        var command = new RegistrarResultadoCommand(
            partidoId,
            2,
            0);

        _partidoRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(
                partidoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partido?)null);

        // Act
        Result resultado = await _handler.Handle(command);

        // Assert
        Assert.True(resultado.IsFailure);
        Assert.Equal("PARTIDO_NO_ENCONTRADO", resultado.ErrorCode);

        _unitOfWorkMock.Verify(
            u => u.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_PartidoSuspendido_DebeRechazarResultado()
    {
        // Arrange
        var partido = CrearPartido();
        partido.CambiarEstado(EstadoPartido.Suspendido);

        var command = new RegistrarResultadoCommand(
            partido.Id,
            2,
            1);

        _partidoRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(
                partido.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(partido);

        // Act
        Result resultado = await _handler.Handle(command);

        // Assert
        Assert.True(resultado.IsFailure);
        Assert.Contains(
            "partido suspendido",
            resultado.ErrorMessage!,
            StringComparison.OrdinalIgnoreCase);

        _partidoRepositoryMock.Verify(
            r => r.Actualizar(It.IsAny<Partido>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            u => u.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_PartidoFinalizado_DebeRechazarSegundoResultado()
    {
        // Arrange
        var partido = CrearPartido();
        partido.RegistrarResultado(1, 0);

        var command = new RegistrarResultadoCommand(
            partido.Id,
            2,
            2);

        _partidoRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(
                partido.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(partido);

        // Act
        Result resultado = await _handler.Handle(command);

        // Assert
        Assert.True(resultado.IsFailure);
        Assert.Contains(
            "ya está finalizado",
            resultado.ErrorMessage!,
            StringComparison.OrdinalIgnoreCase);

        Assert.Equal(1, partido.GolesLocal);
        Assert.Equal(0, partido.GolesVisitante);

        _unitOfWorkMock.Verify(
            u => u.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Partido CrearPartido()
    {
        return new Partido(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1));
    }
}
