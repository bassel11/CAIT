namespace TaskApplication.Dtos
{
    public record TaskDetailsDto(
        Guid Id,
        string Title,
        string Description,
        string Status,       // e.g., "In Progress", "Completed"
        string Priority,     // e.g., "High", "Critical"
        string Category,     // e.g., "Strategic", "Operational"
        DateTime? Deadline,
        List<TaskAssigneeDto> Assignees);


}
