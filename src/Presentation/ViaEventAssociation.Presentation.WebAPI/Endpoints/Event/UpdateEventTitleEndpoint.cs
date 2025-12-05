using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebAPI.Endpoints.Event;

public sealed record UpdateTitleRequest(
    [FromRoute] string Id,
    [FromBody] UpdateTitleBody Body);

public sealed record UpdateTitleBody(string Title);

public sealed class UpdateEventTitleEndpoint(ICommandDispatcher dispatcher)
    : ApiEndpoint
        .WithRequest<UpdateTitleRequest>
        .AndResults<NoContent, BadRequest<IEnumerable<Error>>>
{
    [HttpPost("events/{Id}/update-title")]
    public override async Task<Results<NoContent, BadRequest<IEnumerable<Error>>>> HandleAsync(
        [FromRoute] UpdateTitleRequest request)
    {
        var cmdResult = UpdateEventTitleCommand.Create(request.Id, request.Body.Title);
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