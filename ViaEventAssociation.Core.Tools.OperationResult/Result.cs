namespace ViaEventAssociation.Core.Tools.OperationResult;
using System.Collections.Generic;
using System.Linq;

public class Result
{
    // used "init" instead of "set" to exclude potential mutability. At the moment of first creation of the class, "set" would do the work. But in the case of adding/changing something in the class later, "init" is safer.
    public bool IsSuccess { get; protected init; }
    public bool IsFailure => !IsSuccess;

    public List<string> ErrorMessages { get; protected init; } = new List<string>();
    //public String? ErrorMessage;
    
    
    public static Result Success() => new Result() { IsSuccess = true };
    
    /**
     * "params" is convenient, and it works that you can use as follows:
     *
     * Result.Failure("Error 1");
     * Result.Failure("Error 1", "Error 2", "Error 3");
     *
     * and the arguments are automatically packed into a string[]
     */
    public static Result Failure(IEnumerable<string> errors)
    {
        var errorList = errors switch
        {
            // if errors is a List<string>...
            List<string> list => list,
            // if it's just a simple array[]
            string[] array => array.ToList(),
            // _ means, that the switch can match anything, f.x. a "null" parameter, f.x. in the case where we forget to set an error message and use "null" instead
            _ => errors.ToList()
        };
        return new Result()
        {
            IsSuccess = false,
            ErrorMessages = errorList
        };
    }
}

public class Result<T> : Result
{
    public T? Payload { get; private init; }
    
    public static Result<T> Success(T value) => new Result<T>
    {
        Payload = value,
        IsSuccess = true,
        ErrorMessages = new List<string>()
    };
    
    // "new" because this method has the same body as in super class, and thus it has to overwrite the superclass method. C# does this by default, but it likes to state it implicitly with the "new" keyword.
    // Failure accepts only IEnumerable<string>, so minimum an "array". It shouldn't be all that big of a deal, because it only needs adding square brackets, f.x. ["error"] instead of "error".
    public new static Result<T> Failure(IEnumerable<string> errors)
    {
        var errorList = errors switch
        {
            // if errors is a List<string>...
            List<string> list => list,
            // if it's just a simple array[]
            string[] array => array.ToList(),
            // _ means, that the switch can match anything, f.x. a "null" parameter, in the case where we forget to set an error message and use "null" instead
            _ => errors.ToList()
        };
        return new Result<T>()
        {
            // =default sets the Payload to a default value of a particular type, f.x. if int, then 0; if bool then false; if string then null.
            // Payload needs to be set to default, because in the case of Failure, there's no valuable payload expected with the Result<T> object.
            Payload = default,
            IsSuccess = false,
            ErrorMessages = errorList
        };
    }
}


public class None {}