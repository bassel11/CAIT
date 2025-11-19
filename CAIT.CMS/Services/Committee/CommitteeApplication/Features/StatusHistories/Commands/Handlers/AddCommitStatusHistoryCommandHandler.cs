using AutoMapper;
using CommitteeApplication.Features.StatusHistories.Commands.Models;
using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommitteeApplication.Features.StatusHistories.Commands.Handlers
{
    public class AddCommitStatusHistoryCommandHandler
        : IRequestHandler<AddCommitStatusHistoryCommand, Guid>
    {

        private readonly IStatusHistoryRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddCommitStatusHistoryCommandHandler> _logger;

        public AddCommitStatusHistoryCommandHandler(
            IStatusHistoryRepository repository,
            IMapper mapper,
            ILogger<AddCommitStatusHistoryCommandHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Guid> Handle(AddCommitStatusHistoryCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<CommitteeStatusHistory>(request);

            await _repository.AddAsync(entity);

            _logger.LogInformation($"Added CommitteeStatusHistory {entity.Id} for Committee {entity.CommitteeId}");

            return entity.Id;
        }
    }
}
