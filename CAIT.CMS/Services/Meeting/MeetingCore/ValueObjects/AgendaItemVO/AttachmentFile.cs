namespace MeetingCore.ValueObjects.AgendaItemVO
{
    public sealed record AttachmentFile
    {
        public string FileName { get; }
        public string FileUrl { get; }
        public string ContentType { get; }

        private AttachmentFile(string fileName, string fileUrl, string contentType)
        {
            FileName = fileName;
            FileUrl = fileUrl;
            ContentType = contentType;
        }

        public static AttachmentFile Create(string fileName, string fileUrl, string contentType)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new DomainException("File Name is required.");
            if (string.IsNullOrWhiteSpace(fileUrl)) throw new DomainException("File URL is required.");
            if (string.IsNullOrWhiteSpace(contentType)) throw new DomainException("Content Type is required.");

            return new(fileName, fileUrl, contentType);
        }
    }
}
