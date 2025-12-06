using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebAPI.Endpoints.Event;

public sealed record UpdateDescriptionRequest(
    [FromRoute] string Id,
    [FromBody] UpdateDescriptionBody Body);

public sealed record UpdateDescriptionBody(string Description);

public sealed class UpdateEventDescriptionEndpoint(ICommandDispatcher dispatcher)
    : ApiEndpoint
        .WithRequest<UpdateDescriptionRequest>
        .AndResults<NoContent, BadRequest<IEnumerable<Error>>>
{
    [HttpPost("events/{Id}/update-description")]
    public override async Task<Results<NoContent, BadRequest<IEnumerable<Error>>>> HandleAsync([FromRoute] UpdateDescriptionRequest request)
    {
        var cmdResult = UpdateEventDescriptionCommand.Create(request.Id, request.Body.Description);
        if (cmdResult.IsFailure)
        {
            return TypedResults.BadRequest(cmdResult.Errors.AsEnumerable());
        }
        
        var result = await dispatcher.DispatchAsync(cmdResult.Payload!);
        
        return result.IsSuccess
            ? TypedResults.NoContent()
            : TypedResults.BadRequest(result.Errors.AsEnumerable());
    }

    
}