using CommitteeApplication.Features.CommitteeMembers.Queries.Models;
using CommitteeApplication.Features.CommitteeMembers.Queries.Results;
using CommitteeApplication.Interfaces.Grpc;
using CommitteeCore.Repositories;
using MediatR;

namespace CommitteeApplication.Features.CommitteeMembers.Queries.Handlers
{
    public class GetCommitteeMembersForIntegrationQueryHandler : IRequestHandler<GetCommitteeMembersForIntegrationQuery, List<CommitteeMemberIntegrationResponse>>
    {
        private readonly ICommitteeMemberRepository _repo;
        private readonly IUserGrpcService _userGrpcService;

        public GetCommitteeMembersForIntegrationQueryHandler(
            ICommitteeMemberRepository repo,
            IUserGrpcService userGrpcService)
        {
            _repo = repo;
            _userGrpcService = userGrpcService;
        }

        public async Task<List<CommitteeMemberIntegrationResponse>> Handle(GetCommitteeMembersForIntegrationQuery request, CancellationToken cancellationToken)
        {
            // 1. جلب الأعضاء النشطين مع أدوارهم من قاعدة البيانات
            // ملاحظة: نحتاج لدالة في الريبوزيتوري تدعم Include للأدوار
            var members = await _repo.GetMembersWithRolesAsync(request.CommitteeId);

            if (!members.Any()) return new List<CommitteeMemberIntegrationResponse>();

            // 2. جلب تفاصيل المستخدمين (الاسم والإيميل) من خدمة الهوية عبر gRPC
            var userIds = members.Select(m => m.UserId).Distinct().ToList();
            var usersInfo = await _userGrpcService.GetUsersByIdsAsync(userIds);
            var usersDict = usersInfo.ToDictionary(u => u.UserId, u => u);

            // 3. بناء النتيجة
            var result = new List<CommitteeMemberIntegrationResponse>();

            foreach (var member in members)
            {
                // البحث عن الدور الحالي الفعال
                var activeRole = member.CommitteeMemberRoles
                    .Where(r => r.IsActive && (r.EndDate == null || r.EndDate > DateTime.UtcNow))
                    .OrderByDescending(r => r.CreatedAt) // نأخذ أحدث دور
                    .FirstOrDefault();

                // منطق تحديد الدور وحق التصويت (Logic Mapping)
                // في الواقع، يجب أن يكون لديك جدول يربط RoleId بخصائص مثل VotingRight
                // هنا سنقوم بمحاكاة المنطق بناءً على افتراض
                var (roleInt, canVote) = MapRoleInfo(activeRole?.RoleId);

                if (usersDict.TryGetValue(member.UserId, out var user))
                {
                    result.Add(new CommitteeMemberIntegrationResponse
                    {
                        UserId = member.UserId,
                        FullName = user.FirstName + " " + user.LastName, // افترضنا وجود الاسم
                        Email = user.Email,
                        CommitteeRoleId = roleInt,
                        HasVotingRight = canVote
                    });
                }
            }

            return result;
        }

        // دالة مساعدة لتحويل Guid Role الخاص بالـ Identity إلى مفاهيم اللجنة
        // هذا مجرد Mock Logic، يجب استبداله بالتحقق الفعلي من جدول الأدوار لديك
        private (int RoleId, bool CanVote) MapRoleInfo(Guid? identityRoleId)
        {
            if (identityRoleId == null) return (4, false); // Default: Member, No Vote

            // هنا يجب مقارنة الـ ID مع ثوابت الأدوار في نظامك
            // مثال افتراضي:
            // if (identityRoleId == RoleConstants.ChairmanId) return (1, true);

            // للتبسيط الآن سنعيد (1 - عضو) و (true - تصويت)
            // *يجب عليك تعديل هذا الجزء ليناسب IDs الأدوار الحقيقية في قاعدة بياناتك*
            return (1, true);
        }
    }
}
