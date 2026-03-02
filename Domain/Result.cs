namespace McpD365Server.Domain;

/// <summary>
/// Represents a domain error with a code and descriptive message.
/// </summary>
public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}

/// <summary>
/// Non-generic result representing success or failure.
/// </summary>
public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("A successful result cannot contain an error.");
        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("A failed result must contain an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result Fail(Error error) => new(false, error);
    public static Result<T> Success<T>(T value) => new(value, true, Error.None);
    public static Result<T> Fail<T>(Error error) => new(default, false, error);
}

/// <summary>
/// Generic result carrying a value on success or an error on failure.
/// </summary>
public class Result<T> : Result
{
    private readonly T? _value;

    internal Result(T? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failed result.");

    public static implicit operator Result<T>(T value) => Success(value);
}
