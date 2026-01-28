namespace MeetingApplication.Features.AgendaTemplates.Queries.Results
{
    public record AgendaTemplateResponse(
        Guid Id,
        string Name,
        string Description,
        List<AgendaTemplateItemResponse> Items
    );

    public record AgendaTemplateItemResponse(
        string Title,
        int DurationMinutes,
        string? Description,
        int SortOrder
    );
}
