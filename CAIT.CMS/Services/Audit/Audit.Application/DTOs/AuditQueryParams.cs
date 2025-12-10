namespace Audit.Application.DTOs
{
    public class AuditQueryParams
    {
        public string? ServiceName { get; set; }
        public string? EntityName { get; set; }
        public string? UserId { get; set; }
        public string? ActionType { get; set; }
        public string? PrimaryKey { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 50;
        public string? SortBy { get; set; } = "timestamp_desc";
    }
}
