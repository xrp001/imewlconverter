namespace ImeWlConverter.Abstractions.Results;

/// <summary>
/// Represents the result of an operation that may succeed or fail.
/// Use instead of throwing exceptions for expected failure cases.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
public readonly struct Result<T>
{
    private readonly T? _value;
    private readonly string? _error;

    private Result(T value)
    {
        _value = value;
        _error = null;
        IsSuccess = true;
    }

    private Result(string error)
    {
        _value = default;
        _error = error;
        IsSuccess = false;
    }

    /// <summary>Whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>The success value. Throws if the result is a failure.</summary>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException($"Cannot access Value on failed result: {_error}");

    /// <summary>The error message. Throws if the result is a success.</summary>
    public string Error => !IsSuccess
        ? _error!
        : throw new InvalidOperationException("Cannot access Error on successful result");

    /// <summary>Creates a successful result.</summary>
    public static Result<T> Success(T value) => new(value);

    /// <summary>Creates a failed result.</summary>
    public static Result<T> Failure(string error) => new(error);

    /// <summary>Implicit conversion from value to successful result.</summary>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>Pattern match on success or failure.</summary>
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure)
        => IsSuccess ? onSuccess(_value!) : onFailure(_error!);
}
