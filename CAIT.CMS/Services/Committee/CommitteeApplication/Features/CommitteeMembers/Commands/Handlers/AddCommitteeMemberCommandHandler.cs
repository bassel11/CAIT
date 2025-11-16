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
            var committeeMemberEntity = _mapper.Map<CommitteeMember>(request);
            var generatedCommitteeMember = await _committeeMemberRepository.AddAsync(committeeMemberEntity);
            _logger.LogInformation($"Committee Member with Id {generatedCommitteeMember.Id} successfully created");
            return generatedCommitteeMember.Id;
        }
    }
}
