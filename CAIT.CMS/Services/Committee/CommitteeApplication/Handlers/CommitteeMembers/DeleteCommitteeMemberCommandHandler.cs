using CommitteeApplication.Commands.CommitteeMembers;
using CommitteeApplication.Exceptions;
using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommitteeApplication.Handlers.CommitteeMembers
{
    public class DeleteCommitteeMemberCommandHandler : IRequestHandler<DeleteCommitteeMemberCommand, Unit>
    {
        #region Fields

        private readonly ICommitteeMemberRepository _committeeMemberRepository;
        private readonly ILogger<DeleteCommitteeMemberCommandHandler> _logger;

        #endregion

        #region Constructor

        public DeleteCommitteeMemberCommandHandler(ICommitteeMemberRepository committeeMemberRepository
                                                  , ILogger<DeleteCommitteeMemberCommandHandler> logger)
        {
            _committeeMemberRepository = committeeMemberRepository;
            _logger = logger;
        }

        #endregion


        #region Actions
        public async Task<Unit> Handle(DeleteCommitteeMemberCommand request, CancellationToken cancellationToken)
        {
            var committeeMemberToDelete = await _committeeMemberRepository.GetByIdAsync(request.Id);
            if (committeeMemberToDelete == null)
            {
                throw new CommitteeMemberNotFoundException(nameof(CommitteeMember), request.Id);
            }
            await _committeeMemberRepository.DeleteAsync(committeeMemberToDelete);
            _logger.LogInformation($"Committee Member with {request.Id} is deleted successfully.");
            return Unit.Value;
        }

        #endregion
    }
}
