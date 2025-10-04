using CommitteeApplication.Commands;
using CommitteeApplication.Exceptions;
using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitteeApplication.Handlers
{
    public class DeleteCommitteeCommandHandler :IRequestHandler<DeleteCommitteeCommand, Unit>
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
                throw new CommitteeNotFoundException(nameof(Committee), request.Id);
            }
            await _committeeRepository.DeleteAsync(committeeToDelete);
            _logger.LogInformation($"Committee with {request.Id} is deleted successfully.");
            return Unit.Value;
        }
    }
}
