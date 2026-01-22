using BuildingBlocks.Shared.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MeetingInfrastructure.Interceptors
{
    public class DispatchDomainEventsInterceptor : SaveChangesInterceptor
    {
        private readonly IPublisher _mediator;

        public DispatchDomainEventsInterceptor(IPublisher mediator)
        {
            _mediator = mediator;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            await DispatchEvents(eventData.Context, cancellationToken);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private async Task DispatchEvents(DbContext? context, CancellationToken ct)
        {
            if (context == null) return;

            // تحديد الكيانات التي لديها أحداث
            var entitiesWithEvents = context.ChangeTracker
                .Entries<IHasDomainEvents>() // كان سابقاً IAggregate
                .Where(x => x.Entity.DomainEvents.Any())
                .Select(x => x.Entity);

            var domainEvents = entitiesWithEvents
                .SelectMany(a => a.DomainEvents)
                .ToList();

            entitiesWithEvents.ToList().ForEach(a => a.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent);
            }
        }
    }
}
