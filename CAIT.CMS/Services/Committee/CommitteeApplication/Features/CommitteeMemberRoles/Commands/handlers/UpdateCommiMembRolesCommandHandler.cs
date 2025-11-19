using AutoMapper;
using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Models;
using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Results;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

public class UpdateCommiMembRolesCommandHandler
    : IRequestHandler<UpdateCommiMembRolesCommand, UpdateCommiMembRolesResult>
{
    private readonly ICommitteeMemberRoleRepository _rolesRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateCommiMembRolesCommandHandler> _logger;

    public UpdateCommiMembRolesCommandHandler(
        ICommitteeMemberRoleRepository rolesRepository,
        IMapper mapper,
        ILogger<UpdateCommiMembRolesCommandHandler> logger)
    {
        _rolesRepository = rolesRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UpdateCommiMembRolesResult> Handle(UpdateCommiMembRolesCommand request, CancellationToken cancellationToken)
    {
        var role = await _rolesRepository.GetByIdAsync(request.Id);
        if (role == null)
            throw new KeyNotFoundException($"CommitteeMemberRole {request.Id} not found");

        // check duplicate ignoring same record
        bool duplicateExists = await _rolesRepository.RoleExistsAsync(role.CommitteeMemberId, request.RoleId, excludeId: role.Id);
        if (duplicateExists)
            throw new InvalidOperationException("The member already has this role assigned.");

        // map allowed fields
        _mapper.Map(request, role);

        await _rolesRepository.UpdateAsync(role);

        _logger.LogInformation($"Updated RoleId to {role.RoleId} for CommitteeMemberRole {role.Id}");

        return new UpdateCommiMembRolesResult
        {
            Id = role.Id,
            CommitteeMemberId = role.CommitteeMemberId,
            UpdatedRoleId = role.RoleId
        };
    }
}
