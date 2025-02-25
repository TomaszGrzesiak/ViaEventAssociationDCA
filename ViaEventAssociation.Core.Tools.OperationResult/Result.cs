namespace ViaEventAssociation.Core.Tools.OperationResult;


public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public List<Error> Errors { get; }

    public Result(bool isSuccess, List<Error>? errors = null)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? new List<Error>();
    }
}