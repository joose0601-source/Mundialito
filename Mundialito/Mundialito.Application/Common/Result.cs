using System;
using System.Collections.Generic;
using System.Text;


namespace Mundialito.Application.Common;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? ErrorMessage { get; }
    public string? ErrorCode { get; }

    protected Result(
        bool isSuccess,
        string? errorMessage,
        string? errorCode)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }

    public static Result Success()
        => new(true, null, null);

    public static Result Failure(
        string errorMessage,
        string? errorCode = null)
        => new(false, errorMessage, errorCode);
}

public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(
        bool isSuccess,
        T? value,
        string? errorMessage,
        string? errorCode)
        : base(isSuccess, errorMessage, errorCode)
    {
        Value = value;
    }

    public static Result<T> Success(T value)
        => new(true, value, null, null);

    public new static Result<T> Failure(
        string errorMessage,
        string? errorCode = null)
        => new(false, default, errorMessage, errorCode);
}
