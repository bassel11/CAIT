using AutoMapper;
using CommitteeApplication.Exceptions;
using CommitteeApplication.Features.Committees.Commands.Models;
using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommitteeApplication.Features.Committees.Commands.Handlers
{
    public class UpdateCommitteeCommandHandler : IRequestHandler<UpdateCommitteeCommand, Unit>
    {
        private readonly ICommitteeRepository _committeeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCommitteeCommandHandler> _logger;

        public UpdateCommitteeCommandHandler(ICommitteeRepository committeeRepository, IMapper mapper, ILogger<UpdateCommitteeCommandHandler> logger)
        {
            _committeeRepository = committeeRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<Unit> Handle(UpdateCommitteeCommand request, CancellationToken cancellationToken)
        {
            var committeeToUpdate = await _committeeRepository.GetByIdAsync(request.Id);
            if (committeeToUpdate == null)
            {
                throw new CommitteeNotFoundException(nameof(Committee), request.Id);
            }
            _mapper.Map(request, committeeToUpdate, typeof(UpdateCommitteeCommand), typeof(Committee));
            await _committeeRepository.UpdateAsync(committeeToUpdate);
            _logger.LogInformation($"Committee {committeeToUpdate.Id} is successfully updated");
            return Unit.Value;
        }
    }
}
