using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Models;
using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Results;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommitteeApplication.Features.CommitteeMemberRoles.Commands.handlers
{
    public class UpdateCommiMembRolesCommandHandler
        : IRequestHandler<UpdateCommiMembRolesCommand, UpdateCommiMembRolesResult>
    {
        private readonly ICommitteeMemberRoleRepository _rolesRepository;
        private readonly ILogger<UpdateCommiMembRolesCommandHandler> _logger;

        public UpdateCommiMembRolesCommandHandler(
            ICommitteeMemberRoleRepository rolesRepository,
            ILogger<UpdateCommiMembRolesCommandHandler> logger)
        {
            _rolesRepository = rolesRepository;
            _logger = logger;
        }

        public async Task<UpdateCommiMembRolesResult> Handle(UpdateCommiMembRolesCommand request, CancellationToken cancellationToken)
        {
            var role = await _rolesRepository.GetByIdAsync(request.Id);
            if (role == null)
                throw new KeyNotFoundException($"CommitteeMemberRole {request.Id} not found");

            role.RoleId = request.RoleId;
            await _rolesRepository.UpdateAsync(role);

            _logger.LogInformation($"Updated RoleId to {request.RoleId} for CommitteeMemberRole {request.Id}");

            return new UpdateCommiMembRolesResult
            {
                Id = role.Id,
                CommitteeMemberId = role.CommitteeMemberId,
                UpdatedRoleId = role.RoleId
            };
        }

    }
}
