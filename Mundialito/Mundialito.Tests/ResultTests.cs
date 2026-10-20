
using Mundialito.Application.Common;
using Xunit;

namespace Mundialito.Tests;

public class ResultTests
{
    [Fact]
    public void Success_DebeIndicarOperacionExitosa()
    {
        // Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.ErrorMessage);
        Assert.Null(result.ErrorCode);
    }

    [Fact]
    public void Failure_DebeIndicarOperacionFallida()
    {
        // Act
        var result = Result.Failure("Ocurrió un error.");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal("Ocurrió un error.", result.ErrorMessage);
        Assert.Null(result.ErrorCode);
    }

    [Fact]
    public void Failure_DebeConservarElCodigoDeError()
    {
        // Act
        var result = Result.Failure(
            "El partido no existe.",
            "PARTIDO_NO_ENCONTRADO");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("El partido no existe.", result.ErrorMessage);
        Assert.Equal("PARTIDO_NO_ENCONTRADO", result.ErrorCode);
    }

    [Fact]
    public void SuccessGenerico_DebeConservarElValor()
    {
        // Arrange
        const string valorEsperado = "Operación completada";

        // Act
        var result = Result<string>.Success(valorEsperado);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(valorEsperado, result.Value);
        Assert.Null(result.ErrorMessage);
        Assert.Null(result.ErrorCode);
    }

    [Fact]
    public void FailureGenerico_DebeConservarElError()
    {
        // Act
        var result = Result<string>.Failure(
            "El equipo no existe.",
            "EQUIPO_NO_ENCONTRADO");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Null(result.Value);
        Assert.Equal("El equipo no existe.", result.ErrorMessage);
        Assert.Equal("EQUIPO_NO_ENCONTRADO", result.ErrorCode);
    }

    [Fact]
    public void SuccessGenerico_DebePermitirUnValorNumerico()
    {
        // Act
        var result = Result<int>.Success(3);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public void Failure_DebeAceptarUnCodigoDeErrorNulo()
    {
        // Act
        var result = Result.Failure("Error de validación.");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Error de validación.", result.ErrorMessage);
        Assert.Null(result.ErrorCode);
    }
}
