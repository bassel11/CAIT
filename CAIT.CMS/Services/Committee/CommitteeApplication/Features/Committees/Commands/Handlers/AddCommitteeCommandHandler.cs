using AutoMapper;
using CommitteeApplication.Features.Committees.Commands.Models;
using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommitteeApplication.Features.Committees.Commands.Handlers
{
    public class AddCommitteeCommandHandler : IRequestHandler<AddCommitteeCommand, Guid>
    {
        private readonly ICommitteeRepository _committeeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddCommitteeCommandHandler> _logger;

        public AddCommitteeCommandHandler(ICommitteeRepository committeeRepository, IMapper mapper, ILogger<AddCommitteeCommandHandler> logger)
        {
            _committeeRepository = committeeRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<Guid> Handle(AddCommitteeCommand request, CancellationToken cancellationToken)
        {
            var committeeEntity = _mapper.Map<Committee>(request);
            var generatedCommittee = await _committeeRepository.AddAsync(committeeEntity);
            _logger.LogInformation($"Committee with Id {generatedCommittee.Id} successfully created");
            return generatedCommittee.Id;
        }
    }
}
