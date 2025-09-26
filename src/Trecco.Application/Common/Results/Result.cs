using System.Diagnostics.CodeAnalysis;

namespace Trecco.Application.Common.Results;

public abstract class ResultBase
{
    public bool IsSuccess { get; protected set; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; protected set; } = Error.None;

    protected ResultBase(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }
}

public class Result : ResultBase
{
    public Result(bool isSuccess, Error error) : base(isSuccess, error) { }

    public static Result Success() => new(true, Error.None);
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result Failure(Error error) => new(false, error);
    public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);

    public Result<T> ToResult<T>()
    {
        return new Result<T>(default, IsSuccess, Error);
    }

    public TOut Match<TOut>(Func<TOut> onSuccess, Func<Result, TOut> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(this);

    public static implicit operator Result(Error error) => Failure(error);
}

public sealed class Result<TValue> : ResultBase
{
    private readonly TValue? _value;

    public Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        _value = value;
    }

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failed result.");

    public static Result<TValue> Success(TValue value) => new(value, true, Error.None);
    public static Result<TValue> Failure(Error error) => new(default, false, error);

    public static implicit operator Result<TValue>(TValue value) => Success(value);
    public static implicit operator Result<TValue>(Error error) => Failure(error);
    public static implicit operator Result<TValue>(Result result) => result.ToResult<TValue>();

    public TOut Match<TOut>(Func<TValue, TOut> onSuccess, Func<Result<TValue>, TOut> onFailure) =>
        IsSuccess ? onSuccess(Value) : onFailure(this);

    public static Result<TValue> ValidationFailure(Error error) =>
        new(default, false, error);
}
