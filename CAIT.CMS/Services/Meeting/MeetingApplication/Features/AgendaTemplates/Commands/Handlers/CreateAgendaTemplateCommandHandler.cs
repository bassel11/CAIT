using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaTemplates.Commands.Models;
using MeetingCore.Entities;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AgendaTemplateVO;

namespace MeetingApplication.Features.AgendaTemplates.Commands.Handlers
{
    public class CreateAgendaTemplateCommandHandler : ICommandHandler<CreateAgendaTemplateCommand, Result<Guid>>
    {
        private readonly IAgendaTemplateRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public CreateAgendaTemplateCommandHandler(
            IAgendaTemplateRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(CreateAgendaTemplateCommand request, CancellationToken ct)
        {
            var template = new AgendaTemplate(
                AgendaTemplateId.Of(Guid.NewGuid()),
                request.Name,
                request.Description,
                _currentUserService.UserId.ToString()
            );

            foreach (var item in request.Items)
            {
                template.AddItem(item.Title, item.DurationMinutes, item.Description, item.SortOrder);
            }

            await _repository.AddAsync(template, ct);
            await _repository.UnitOfWork.SaveChangesAsync(ct);

            return Result<Guid>.Success(template.Id.Value, "Template created.");
        }
    }
}
