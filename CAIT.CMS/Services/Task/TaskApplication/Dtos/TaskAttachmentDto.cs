namespace TaskApplication.Dtos
{
    public record TaskAttachmentDto(
        Guid Id,
        string FileName,
        string FileUrl,       // الرابط الكامل للتحميل (Download Link)
        string ContentType,   // e.g., "application/pdf"
        long SizeInBytes,
        int Version,
        Guid UploadedByUserId,
        string UploadedByName, // اسم من قام بالرفع (للعرض)
        DateTime UploadedAt
    );
}
