namespace ViaEventAssociation.Core.Tools.ObjectMapper;

public interface IObjectMapper
{
    TOutput Map<TOutput>(object inpout)
    where TOutput : class;
}