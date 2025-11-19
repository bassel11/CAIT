using AutoMapper;
using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Models;
using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Results;
using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

public class AddCommiMembRolesCommandHandler
    : IRequestHandler<AddCommiMembRolesCommand, AddCommiMembRolesResult>
{
    private readonly ICommitteeMemberRoleRepository _rolesRepository;
    private readonly ICommitteeMemberRepository _memberRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AddCommiMembRolesCommandHandler> _logger;

    public AddCommiMembRolesCommandHandler(
        ICommitteeMemberRoleRepository rolesRepository,
        ICommitteeMemberRepository memberRepository,
        IMapper mapper,
        ILogger<AddCommiMembRolesCommandHandler> logger)
    {
        _rolesRepository = rolesRepository;
        _memberRepository = memberRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AddCommiMembRolesResult> Handle(AddCommiMembRolesCommand request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(request.CommitteeMemberId);
        if (member == null)
            throw new KeyNotFoundException("Committee member not found");

        // bulk check existing role ids
        var existingRoleIds = await _rolesRepository.GetRoleIdsByMemberIdAsync(member.Id);

        var newRoleIds = request.RoleIds.Except(existingRoleIds).Distinct().ToList();
        var ignoredRoleIds = request.RoleIds.Except(newRoleIds).ToList();

        var rolesToAdd = newRoleIds
            .Select(rid => _mapper.Map<CommitteeMemberRole>(new SingleCommiMembRoleItem
            {
                CommitteeMemberId = member.Id,
                RoleId = rid
            }))
            .ToList();

        if (rolesToAdd.Any())
        {
            await _rolesRepository.AddRolesAsync(rolesToAdd);
            _logger.LogInformation($"Added {rolesToAdd.Count} new roles for member {member.Id}");
        }

        return new AddCommiMembRolesResult
        {
            CommitteeMemberId = member.Id,
            AddedRoleIds = newRoleIds,
            IgnoredRoleIds = ignoredRoleIds
        };
    }
}
