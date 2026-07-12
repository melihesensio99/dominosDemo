namespace BuildingBlocks.Common;

public enum ErrorType
{
    Failure,
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
}

public sealed record Error(string Code, string Message, ErrorType Type = ErrorType.Failure);

public sealed record Result(bool IsSuccess, Error? Error)
{
    public static Result Success() => new(true, null);

    public static Result Failure(string code, string message) => new(false, new Error(code, message));

    public static Result Validation(string code, string message) => new(false, new Error(code, message, ErrorType.Validation));

    public static Result NotFound(string code, string message) => new(false, new Error(code, message, ErrorType.NotFound));

    public static Result Conflict(string code, string message) => new(false, new Error(code, message, ErrorType.Conflict));

    public static Result Unauthorized(string code, string message) => new(false, new Error(code, message, ErrorType.Unauthorized));

    public static Result Forbidden(string code, string message) => new(false, new Error(code, message, ErrorType.Forbidden));
}

public sealed record Result<T>(bool IsSuccess, T? Value, Error? Error)
{
    public static Result<T> Success(T value) => new(true, value, null);

    public static Result<T> Failure(string code, string message) => new(false, default, new Error(code, message));

    public static Result<T> Validation(string code, string message) => new(false, default, new Error(code, message, ErrorType.Validation));

    public static Result<T> NotFound(string code, string message) => new(false, default, new Error(code, message, ErrorType.NotFound));

    public static Result<T> Conflict(string code, string message) => new(false, default, new Error(code, message, ErrorType.Conflict));

    public static Result<T> Unauthorized(string code, string message) => new(false, default, new Error(code, message, ErrorType.Unauthorized));

    public static Result<T> Forbidden(string code, string message) => new(false, default, new Error(code, message, ErrorType.Forbidden));
}
