using AutoMapper;
using CommitteeApplication.Commands.CommitteeMembers;
using CommitteeApplication.Exceptions;
using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommitteeApplication.Handlers.CommitteeMembers
{
    public class UpdateCommitteeMemberCommandHandler : IRequestHandler<UpdateCommitteeMemberCommand, Unit>
    {
        #region Fields
        private readonly ICommitteeMemberRepository _committeeMemberRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCommitteeMemberCommandHandler> _logger;

        #endregion

        #region Constructor

        public UpdateCommitteeMemberCommandHandler(ICommitteeMemberRepository committeeMemberRepository
                                                  , IMapper mapper
                                                  , ILogger<UpdateCommitteeMemberCommandHandler> logger)
        {
            _committeeMemberRepository = committeeMemberRepository;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion

        #region Actions
        public async Task<Unit> Handle(UpdateCommitteeMemberCommand request, CancellationToken cancellationToken)
        {
            var commitMemberToUpdate = await _committeeMemberRepository.GetByIdAsync(request.Id);
            if (commitMemberToUpdate == null)
            {
                throw new CommitteeMemberNotFoundException(nameof(CommitteeMember), request.Id);
            }
            _mapper.Map(request, commitMemberToUpdate, typeof(UpdateCommitteeMemberCommand), typeof(CommitteeMember));
            await _committeeMemberRepository.UpdateAsync(commitMemberToUpdate);
            _logger.LogInformation($"Committee Member {commitMemberToUpdate.Id} is successfully updated");
            return Unit.Value;
        }

        #endregion
    }
}
