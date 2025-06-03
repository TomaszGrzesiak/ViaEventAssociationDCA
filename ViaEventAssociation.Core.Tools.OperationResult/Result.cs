namespace ViaEventAssociation.Core.Tools.OperationResult;

using System.Collections.Generic;
using System.Linq;

public class Result
{
    // used "init" and "IReadOnlyList" instead of "set" and "List" to exclude potential mutability.
    public IReadOnlyList<Error> Errors { get; protected init; } = new List<Error>();
    public bool IsSuccess => Errors.Count == 0;
    public bool IsFailure => !IsSuccess;

    public static Result Success() => new Result();

    // params allows to use Result.Failure(error1, error2, error3); instead of Result.Failure(new[] { error1, error2, error3 });
    public static Result Failure(params Error[] errors) => new Result() { Errors = errors.ToList() };

    public static Result Combine(Result original, params Error[] errors)
    {
        var combined = original.Errors.Concat(errors).ToList();
        return new Result { Errors = combined };
    }
}

public class Result<T> : Result
{
    public T? Payload { get; private init; }

    private Result(List<Error> errors)
    {
        if (errors == null || errors.Count == 0)
            throw new ArgumentException("Failure must contain at least one error.");
        Payload = default; // default sets the value to 0, null or other default value of the particular object
        Errors = errors;
    }

    private Result(T value)
    {
        Payload = value;
        Errors = new List<Error>();
    }

    public static Result<T> Success(T value) => new Result<T>(value);

    public static Result<T> Failure(Error error) =>
        new Result<T>([error]);

    public new static Result<T> Failure(params Error[] errors) =>
        new Result<T>(errors.ToList());
}