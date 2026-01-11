using BuildingBlocks.Shared.Exceptions;
using CommitteeApplication.Features.Committees.Commands.Models;
using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommitteeApplication.Features.Committees.Commands.Handlers
{
    public class DeleteCommitteeCommandHandler : IRequestHandler<DeleteCommitteeCommand, Unit>
    {
        private readonly ICommitteeRepository _committeeRepository;
        private readonly ILogger<DeleteCommitteeCommandHandler> _logger;

        public DeleteCommitteeCommandHandler(ICommitteeRepository committeeRepository, ILogger<DeleteCommitteeCommandHandler> logger)
        {
            _committeeRepository = committeeRepository;
            _logger = logger;
        }
        public async Task<Unit> Handle(DeleteCommitteeCommand request, CancellationToken cancellationToken)
        {
            var committeeToDelete = await _committeeRepository.GetByIdAsync(request.Id);
            if (committeeToDelete == null)
            {
                throw new NotFoundException(nameof(Committee), request.Id);
            }
            await _committeeRepository.DeleteAsync(committeeToDelete);
            _logger.LogInformation($"Committee with {request.Id} is deleted successfully.");
            return Unit.Value;
        }
    }
}
