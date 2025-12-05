namespace ViaEventAssociation.Core.Tools.ObjectMapper;

public interface IMappingConfig<TInput, TOutput>
    where TInput : class
    where TOutput : class
{
    TOutput Map(TInput input);
}