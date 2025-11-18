using CommitteeApplication.Features.CommitteeMembers.Commands.Models;
using CommitteeApplication.Features.CommitteeMembers.Commands.Results;
using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommitteeApplication.Features.CommitteeMembers.Commands.Handlers
{

    public class AssignCommitteeMembersCommandHandler
    : IRequestHandler<AssignCommitteeMembersCommand, AssignCommitteeMembersResult>
    {
        private readonly ICommitteeMemberRepository _committeeMemberRepository;
        private readonly ILogger<AssignCommitteeMembersCommandHandler> _logger;

        public AssignCommitteeMembersCommandHandler(
            ICommitteeMemberRepository committeeMemberRepository,
            ILogger<AssignCommitteeMembersCommandHandler> logger)
        {
            _committeeMemberRepository = committeeMemberRepository;
            _logger = logger;
        }

        public async Task<AssignCommitteeMembersResult> Handle(AssignCommitteeMembersCommand request, CancellationToken cancellationToken)
        {
            var result = new AssignCommitteeMembersResult();
            var newMembersToAdd = new List<CommitteeMember>();

            // HashSet لتتبع UserIds المضافة في نفس الطلب
            var userIdsInRequest = new HashSet<Guid>();

            foreach (var member in request.Members)
            {
                // تحقق من تكرار UserId داخل نفس الطلب
                if (!userIdsInRequest.Add(member.UserId))
                {
                    _logger.LogWarning($"Duplicate UserId {member.UserId} in request skipped.");
                    continue;
                }

                bool exists = await _committeeMemberRepository
                    .IsMemberExistsAsync(request.CommitteeId, member.UserId);

                if (exists)
                {
                    _logger.LogInformation($"User {member.UserId} already exists in committee {request.CommitteeId}");
                    continue;
                }

                var committeeMember = new CommitteeMember
                {
                    CommitteeId = request.CommitteeId,
                    UserId = member.UserId,
                    Affiliation = member.Affiliation,
                    ContactDetails = member.ContactDetails,
                    //CommitteeMemberRoles = member.RoleIds
                    //    .Select(roleId => new CommitteeMemberRole
                    //    {
                    //        RoleId = roleId
                    //    })
                    //    .ToList()
                };

                newMembersToAdd.Add(committeeMember);
            }

            // إذا لا يوجد أعضاء جدد، أرجع نتيجة فارغة
            if (!newMembersToAdd.Any())
            {
                _logger.LogInformation("No new members to add.");
                return result;
            }

            // إضافة الأعضاء دفعة واحدة
            await _committeeMemberRepository.AddRangeAsync(newMembersToAdd);

            // إرجاع IDs التي تمت إضافتها
            result.AddedMemberIds = newMembersToAdd.Select(x => x.Id).ToList();

            _logger.LogInformation($"{result.AddedMemberIds.Count} members added to committee {request.CommitteeId}");

            return result;
        }
    }


}
