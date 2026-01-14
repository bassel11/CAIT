using AutoMapper;
using BuildingBlocks.Shared.Exceptions;
using CommitteeApplication.Features.CommitteeMembers.Commands.Models;
using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommitteeApplication.Features.CommitteeMembers.Commands.Handlers
{
    public class AddCommitteeMemberCommandHandler : IRequestHandler<AddCommitteeMemberCommand, Guid>
    {
        #region Fields

        private readonly ICommitteeMemberRepository _committeeMemberRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddCommitteeMemberCommandHandler> _logger;

        #endregion

        #region Constructor

        public AddCommitteeMemberCommandHandler(ICommitteeMemberRepository committeeMemberRepository
                                              , IMapper mapper
                                              , ILogger<AddCommitteeMemberCommandHandler> logger)
        {
            _committeeMemberRepository = committeeMemberRepository;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion

        public async Task<Guid> Handle(AddCommitteeMemberCommand request, CancellationToken cancellationToken)
        {
            // تحقق من وجود العضو مسبقًا
            bool exists = await _committeeMemberRepository.IsMemberExistsAsync(request.CommitteeId, request.UserId);
            if (exists)
            {
                _logger.LogWarning($"User {request.UserId} already exists in committee {request.CommitteeId}");
                throw new DomainException("This user is already assigned to the committee.");
            }

            // يجب جلب بيانات المستخدم من Identity like userfullname and useremail

            // إذا لم يكن موجودًا، أضف العضو
            var committeeMemberEntity = _mapper.Map<CommitteeMember>(request);

            var actualJoinDate = request.JoinDate ?? DateTime.UtcNow;
            committeeMemberEntity.JoinDate = actualJoinDate;
            committeeMemberEntity.LeaveDate = request.LeaveDate;
            committeeMemberEntity.IsActive = request.IsActive;

            if (request.RoleIds != null && request.RoleIds.Any())
            {
                foreach (var roleId in request.RoleIds)
                {
                    committeeMemberEntity.CommitteeMemberRoles.Add(new CommitteeMemberRole
                    {
                        Id = Guid.NewGuid(),
                        RoleId = roleId,
                        StartDate = DateTime.UtcNow
                    });
                }
            }
            var generatedCommitteeMember = await _committeeMemberRepository.AddAsync(committeeMemberEntity);

            _logger.LogInformation($"Committee Member with Id {generatedCommitteeMember.Id} successfully created");
            return generatedCommitteeMember.Id;
        }
    }
}
