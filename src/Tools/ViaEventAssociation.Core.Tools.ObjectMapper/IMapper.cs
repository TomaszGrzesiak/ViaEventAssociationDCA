namespace ViaEventAssociation.Core.Tools.ObjectMapper;

public interface IMapper // TROELS' interface proposition
{
        TOutput Map<TOutput>(object input) where TOutput : class;
}