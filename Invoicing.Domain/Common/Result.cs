using System.Collections.ObjectModel;

namespace Invoicing.Domain.Common;

/// <summary>
/// Outcome of a use case without any HTTP details attached.
/// Return Failure(...) for expected problems (validation, not found, etc.).
/// Throw exceptions only for unexpected bugs or infrastructure failures.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public IReadOnlyList<Error> Errors { get; }

    protected Result(bool success, IEnumerable<Error>? errors = null)
    {
        IsSuccess = success;
        Errors = new ReadOnlyCollection<Error>((errors ?? Array.Empty<Error>()).ToList());
    }

    public static Result Success() => new(true);
    public static Result Failure(params Error[] errors) => new(false, errors);
    public static Result Failure(IEnumerable<Error> errors) => new(false, errors);
}

/// <summary>
/// Same as Result, but carries a value when successful.
/// </summary>
public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool success, T? value, IEnumerable<Error>? errors = null)
        : base(success, errors)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value);
    public static Result<T> Failure(params Error[] errors) => new(false, default, errors);
    public static Result<T> Failure(IEnumerable<Error> errors) => new(false, default, errors);
}