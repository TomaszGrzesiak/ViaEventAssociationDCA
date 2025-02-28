namespace ViaEventAssociation.Core.Tools.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;

public class Result
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Code { get; }
    public string Message { get; }

    public Result(string code, string message)
    {
        Code = code;
        Message = message;
    }
    
    
}

// public class Result<T> : Result
// {
//     public T Value { get; }
//
//     public Result(T value, bool isSuccess, List<Error>? errors = null)
//         : base(isSuccess, errors)
//     {
//         Value = value;
//     }
// }