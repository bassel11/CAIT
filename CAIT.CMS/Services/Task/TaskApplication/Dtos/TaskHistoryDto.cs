namespace TaskApplication.Dtos
{
    public record TaskHistoryDto(
        Guid Id,
        string Action,        // e.g., "StatusChanged", "Assigned"
        string Details,       // e.g., "Changed status from Open to InProgress"
        Guid UserId,          // من قام بالفعل
        string UserName,      // اسم الفاعل
        DateTime Timestamp
    );
}
