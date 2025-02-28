namespace ViaEventAssociation.Core.Tools.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;

public class Result
{
    public Guid Id { get; } = Guid.NewGuid();
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public List<Error> Errors { get; }

    public Result(bool isSuccess, List<Error>? errors = null)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? new List<Error>();
    }

    public static Result Success() => new Result(true);

    public static Result Failure(Error[] errors) => new Result(false, errors.ToList());
    
}

public class Result<T> : Result
{
    public T Value { get; }

    public Result(T value, bool isSuccess, List<Error>? errors = null)
        : base(isSuccess, errors)
    {
        Value = value;
    }
}