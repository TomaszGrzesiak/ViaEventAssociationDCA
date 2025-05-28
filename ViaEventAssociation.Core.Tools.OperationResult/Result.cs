namespace ViaEventAssociation.Core.Tools.OperationResult;
using System.Collections.Generic;
using System.Linq;

public class Result
{
    // used "init" instead of "set" to exclude potential mutability. At the moment of first creation of the class, "set" would do the work. But in the case of adding/changing something in the class later, "init" is safer.
    public bool IsSuccess { get; protected init; }
    public bool IsFailure => !IsSuccess;

    public List<string> ErrorMessages { get; protected init; }
    
    // constructor for Failure
    protected Result(List<string> errorMessages)
    {
        IsSuccess = false;
        ErrorMessages = errorMessages;
    }
    
    // Constructor for success
    protected Result()
    {
        IsSuccess = true;
        ErrorMessages = new List<string>();
    }
     
    // factory methods:
    public static Result Success() => new Result();
    
    public static Result Failure(string errorMessage) =>
        new Result(new List<string> { errorMessage });

    public static Result Failure(params string[] errorMessages) =>
        new Result(errorMessages.ToList());

    public static Result Failure(List<string> errorMessages) =>
        new Result(errorMessages);
}

public class Result<T> : Result
{
    public T? Payload { get; private init; }

    // constructor for Failure. Payload in Failure will probably always be null or some sort of empty object.
    private Result(List<string> errorMessages, T? payload = default) : base(errorMessages)
    {
        Payload = payload;
    }

    private Result(T payload) : base() // this method expands on the base "private Result()" method from the class Result
    {
        Payload = payload;
    }

    // factory methods:
    public static Result<T> Success(T value) => new Result<T>(value); 
    
    // "new" because this method has the same body as in super class, and thus it has to overwrite the superclass method. C# does this by default, but it likes to state it implicitly with the "new" keyword.
    // Failure accepts only IEnumerable<string>, so minimum an "array". It shouldn't be all that big of a deal, because it only needs adding square brackets, f.x. ["error"] instead of "error".
    public new static Result<T> Failure(string errorMessage) =>
        new Result<T>(new List<string> { errorMessage });

    public new static Result<T> Failure(params string[] errorMessages) =>
        new Result<T>(errorMessages.ToList());

    public new static Result<T> Failure(List<string> errorMessages) =>
        new Result<T>(errorMessages);
}


public class None {}