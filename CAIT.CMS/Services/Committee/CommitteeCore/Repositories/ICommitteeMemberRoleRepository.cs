using CommitteeCore.Entities;
using CommitteeCore.Repositories;

public interface ICommitteeMemberRoleRepository : IAsyncRepository<CommitteeMemberRole>
{
    // استرجاع كل Roles لعضو معين
    Task<List<CommitteeMemberRole>> GetRolesByMemberIdAsync(Guid committeeMemberId);

    // استرجاع Ids فقط للتحقق من التكرار Bulk
    Task<List<Guid>> GetRoleIdsByMemberIdAsync(Guid committeeMemberId);

    // إضافة عدة Roles دفعة واحدة
    Task AddRolesAsync(IEnumerable<CommitteeMemberRole> roles);

    // حذف عدة Roles دفعة واحدة
    Task RemoveRolesAsync(IEnumerable<CommitteeMemberRole> roles);

    // التحقق من وجود Role معين لعضو مع استثناء Id (لـ Update)
    Task<bool> RoleExistsAsync(Guid committeeMemberId, Guid roleId, Guid? excludeId = null);
}
