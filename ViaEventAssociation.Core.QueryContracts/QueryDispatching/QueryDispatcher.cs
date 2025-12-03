using ViaEventAssociation.Core.QueryContracts.Contracts;

namespace ViaEventAssociation.Core.QueryContracts.QueryDispatching;

public class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
{
     public Task<TAnswer> DispatchAsync<TAnswer>(IQuery<TAnswer> query) 
     {
          Type queryInterfaceWithTypes = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TAnswer));
          dynamic handler = serviceProvider.GetService(queryInterfaceWithTypes)!;

          if (handler == null)
          {
               //throw new QueryHandlerNotFoundException(query.GetType().ToString(), typeof(TAnswer).ToString());
               throw new InvalidOperationException(
                    $"No query handler registered for query '{query.GetType().Name}' with answer type '{typeof(TAnswer).Name}'.");
          }

          return handler.HandleAsync((dynamic)query);
     }
}
