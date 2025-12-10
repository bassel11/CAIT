using Audit.Domain.Entities;

namespace Audit.Application.DTOs
{
    public record AuditQueryResult(
        int Total,
        List<AuditLog> Items
    );
}
