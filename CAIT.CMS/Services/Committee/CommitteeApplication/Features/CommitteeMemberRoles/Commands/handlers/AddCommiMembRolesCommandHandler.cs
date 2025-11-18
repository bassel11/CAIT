using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Models;
using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Results;
using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommitteeApplication.Features.CommitteeMemberRoles.Commands.handlers
{
    public class AddCommiMembRolesCommandHandler
        : IRequestHandler<AddCommiMembRolesCommand, AddCommiMembRolesResult>
    {
        private readonly ICommitteeMemberRoleRepository _rolesRepository;
        private readonly ICommitteeMemberRepository _memberRepository;
        private readonly ILogger<AddCommiMembRolesCommandHandler> _logger;

        public AddCommiMembRolesCommandHandler(
            ICommitteeMemberRoleRepository rolesRepository,
            ICommitteeMemberRepository memberRepository,
            ILogger<AddCommiMembRolesCommandHandler> logger)
        {
            _rolesRepository = rolesRepository;
            _memberRepository = memberRepository;
            _logger = logger;
        }

        public async Task<AddCommiMembRolesResult> Handle(AddCommiMembRolesCommand request, CancellationToken cancellationToken)
        {
            var member = await _memberRepository.GetByIdAsync(request.CommitteeMemberId);

            if (member == null)
                throw new KeyNotFoundException("Committee member not found");

            // تجاهل الأدوار المكررة
            var existingRoles = await _rolesRepository.GetRolesByMemberIdAsync(member.Id);

            var rolesToAdd = request.RoleIds
                .Where(rid => !existingRoles.Any(er => er.RoleId == rid))
                .Select(rid => new CommitteeMemberRole
                {
                    CommitteeMemberId = member.Id,
                    RoleId = rid
                })
                .ToList();

            if (rolesToAdd.Any())
                await _rolesRepository.AddRolesAsync(rolesToAdd);

            return new AddCommiMembRolesResult
            {
                CommitteeMemberId = member.Id,
                AddedRoleIds = rolesToAdd.Select(r => r.RoleId).ToList()
            };
        }
    }
}
