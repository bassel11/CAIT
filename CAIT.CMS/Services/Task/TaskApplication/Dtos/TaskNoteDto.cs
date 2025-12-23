namespace TaskApplication.Dtos
{
    public record TaskNoteDto(
        Guid Id,
        string Content,
        Guid UserId,
        DateTime? CreatedAt
    );
}
