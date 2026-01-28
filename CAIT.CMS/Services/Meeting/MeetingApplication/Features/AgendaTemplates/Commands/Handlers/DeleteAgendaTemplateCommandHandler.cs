using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaTemplates.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AgendaTemplateVO;

namespace MeetingApplication.Features.AgendaTemplates.Commands.Handlers
{
    public class DeleteAgendaTemplateCommandHandler : ICommandHandler<DeleteAgendaTemplateCommand, Result>
    {
        private readonly IAgendaTemplateRepository _repository;

        public DeleteAgendaTemplateCommandHandler(IAgendaTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(DeleteAgendaTemplateCommand request, CancellationToken ct)
        {
            var template = await _repository.GetByIdAsync(AgendaTemplateId.Of(request.Id), ct);
            if (template == null) return Result.Failure("Template not found.");

            _repository.Delete(template);
            await _repository.UnitOfWork.SaveChangesAsync(ct);

            return Result.Success("Template deleted.");
        }
    }
}
