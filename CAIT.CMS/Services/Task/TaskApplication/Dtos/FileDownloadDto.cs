namespace TaskApplication.Dtos
{
    public record FileDownloadDto(Stream FileStream, string ContentType, string FileName);
}
