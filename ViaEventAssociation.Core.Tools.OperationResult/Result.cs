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
    
    
    private Result(List<string>? errorMessages)
    {
        Payload = default; // default sets the value to 0, null or other default value of the particular object
        IsSuccess = false;
        ErrorMessages = errorMessages ?? ["Unknown error. This message should never be displayed."];
    }
    
    public static Result<T> Success(T value) => new Result<T>(null)
    {
        Payload = value,
        IsSuccess = true,
        ErrorMessages = new List<string>()
    };
    
    // "new" because this method has the same body as in super class, and thus it has to overwrite the superclass method. C# does this by default, but it likes to state it implicitly with the "new" keyword.
    // Failure accepts only IEnumerable<string>, so minimum an "array". It shouldn't be all that big of a deal, because it only needs adding square brackets, f.x. ["error"] instead of "error".
    public static Result<T> Failure(string errorMessage) =>
        new Result<T>(new List<string> { errorMessage });

    public static Result<T> Failure(params string[] errorMessages) =>
        new Result<T>(errorMessages.ToList());

    public static Result<T> Failure(List<string> errorMessages) =>
        new Result<T>(errorMessages);
}


public class None {}