using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Models;
using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Results;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommitteeApplication.Features.CommitteeMemberRoles.Commands.handlers
{
    public class DeleteCommiMembRolesCommandHandler
        : IRequestHandler<DeleteCommiMembRolesCommand, DeleteCommiMembRolesResult>
    {
        private readonly ICommitteeMemberRoleRepository _rolesRepository;
        private readonly ILogger<DeleteCommiMembRolesCommandHandler> _logger;

        public DeleteCommiMembRolesCommandHandler(
            ICommitteeMemberRoleRepository rolesRepository,
            ILogger<DeleteCommiMembRolesCommandHandler> logger)
        {
            _rolesRepository = rolesRepository;
            _logger = logger;
        }

        public async Task<DeleteCommiMembRolesResult> Handle(DeleteCommiMembRolesCommand request, CancellationToken cancellationToken)
        {
            var role = await _rolesRepository.GetByIdAsync(request.Id);
            if (role == null)
                throw new KeyNotFoundException($"CommitteeMemberRole {request.Id} not found");

            await _rolesRepository.DeleteAsync(role);

            _logger.LogInformation($"Deleted CommitteeMemberRole {request.Id}");

            return new DeleteCommiMembRolesResult
            {
                Id = role.Id,
                CommitteeMemberId = role.CommitteeMemberId,
                DeletedRoleId = role.RoleId
            };
        }
    }
}
