namespace TaskApplication.Dtos
{
    public record TaskDto(
        Guid Id,
        string Title,
        string Description,
        string Status,
        string Priority,
        DateTime? Deadline,
        List<TaskAssigneeDto> Assignees
    );

    public record TaskAssigneeDto(Guid UserId, string Name, string Email);
}
