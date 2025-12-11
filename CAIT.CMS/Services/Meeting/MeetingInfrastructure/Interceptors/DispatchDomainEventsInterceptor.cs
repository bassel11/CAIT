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
            var entities = context.ChangeTracker
                .Entries<MeetingCore.Entities.MinutesOfMeeting>() // أو BaseEntity
                .Where(e => e.Entity.Events.Any())
                .Select(e => e.Entity)
                .ToList();

            if (!entities.Any()) return;

            var domainEvents = entities.SelectMany(e => e.Events).ToList();

            entities.ForEach(e => e.ClearEvents());

            // نشر الأحداث محلياً لـ Handlers (Audit, Integration)
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, ct);
            }
        }
    }
}
