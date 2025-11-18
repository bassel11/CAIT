using AutoMapper;
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
                throw new InvalidOperationException("This user is already assigned to the committee.");
            }

            // إذا لم يكن موجودًا، أضف العضو
            var committeeMemberEntity = _mapper.Map<CommitteeMember>(request);
            var generatedCommitteeMember = await _committeeMemberRepository.AddAsync(committeeMemberEntity);

            _logger.LogInformation($"Committee Member with Id {generatedCommitteeMember.Id} successfully created");
            return generatedCommitteeMember.Id;
        }
    }
}
