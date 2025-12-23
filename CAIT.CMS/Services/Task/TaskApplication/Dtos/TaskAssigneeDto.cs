namespace TaskApplication.Dtos
{
    public record TaskAssigneeDto(
        Guid UserId,
        string Name,
        string Email
    );
}
