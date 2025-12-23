namespace TaskApplication.Dtos
{
    public record TaskListItemDto(
        Guid Id,
        string Title,
        string Status,
        string Priority,
        string Category,
        DateTime? Deadline,
        Guid CommitteeId,
        DateTime? CreatedAt
    );
}
