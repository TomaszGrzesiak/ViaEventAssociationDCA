using Application.AppEntry;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using ViaEventAssociation.Core.Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebAPI.Endpoints.Event;

public sealed record CreateEventResponse(string Id);

public sealed class CreateEventEndpoint(ICommandDispatcher dispatcher)
    : ApiEndpoint
        .WithoutRequest
        .AndResults<Ok<CreateEventResponse>, BadRequest<IEnumerable<Error>>>
{
    [HttpPost("events/create")]
    public override async Task<Results<Ok<CreateEventResponse>, BadRequest<IEnumerable<Error>>>> HandleAsync()
    {
        var newId = Guid.NewGuid().ToString();

        var cmdResult = CreateEventCommand.Create(newId);
        if (cmdResult.IsFailure)
        {
            return TypedResults.BadRequest(cmdResult.Errors.AsEnumerable());
        }

        var result = await dispatcher.DispatchAsync(cmdResult.Payload!);
        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Errors.AsEnumerable());
        }

        var response = new CreateEventResponse(newId);
        return TypedResults.Ok(response);
    }
}
