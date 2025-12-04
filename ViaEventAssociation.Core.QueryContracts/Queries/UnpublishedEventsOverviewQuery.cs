using ViaEventAssociation.Core.QueryContracts.Contracts;

namespace ViaEventAssociation.Core.QueryContracts.Queries;

public static class UnpublishedEventsOverviewQuery
{
    // ---- QUERY ----
    // No parameters for now – this is a global overview.
    public sealed record Query() : IQuery<Answer>;

    // ---- ANSWER ----
    public sealed record Answer(
        IReadOnlyCollection<EventDto> Drafts,
        IReadOnlyCollection<EventDto> Ready,
        IReadOnlyCollection<EventDto> Cancelled);

    // ---- DTO ----
    public sealed record EventDto(
        string EventId,
        string Title);
}