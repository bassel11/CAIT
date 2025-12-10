using Audit.Application.Contracts;
using Audit.Domain.Entities;
using Audit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Audit.Infrastructure.Services
{
    public class AuditStore : IAuditStore
    {
        private readonly AuditDbContext _db;

        public AuditStore(AuditDbContext db) => _db = db;

        public async Task AppendAsync(AuditLog log, CancellationToken ct = default)
        {
            // compute previous hash
            var previous = await _db.AuditLogs.OrderByDescending(x => x.ReceivedAt).FirstOrDefaultAsync(ct);
            log.PreviousHash = previous?.Hash;

            // compute current hash
            log.Hash = ComputeHash(log);

            _db.AuditLogs.Add(log);
            await _db.SaveChangesAsync(ct);
        }

        private static string ComputeHash(AuditLog log)
        {
            using var sha = SHA256.Create();
            var input = $"{log.EventId}|{log.ServiceName}|{log.EntityName}|{log.ActionType}|{log.PrimaryKey}|{log.OldValues}|{log.NewValues}|{log.Timestamp:o}|{log.PreviousHash}";
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }
    }
}
