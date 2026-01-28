using MeetingCore.Entities;
using MeetingCore.ValueObjects.AgendaTemplateVO;

namespace MeetingCore.Repositories
{
    public interface IAgendaTemplateRepository
    {
        IUnitOfWork UnitOfWork { get; }

        Task AddAsync(AgendaTemplate template, CancellationToken ct = default);

        // جلب القالب مع بنوده (للتعديل أو العرض التفصيلي)
        Task<AgendaTemplate?> GetByIdAsync(AgendaTemplateId id, CancellationToken ct = default);

        // جلب القائمة للعرض (بدون تفاصيل البنود للأداء - Projection)
        Task<List<AgendaTemplate>> GetAllAsync(CancellationToken ct = default);

        // حذف
        void Delete(AgendaTemplate template);
    }
}
