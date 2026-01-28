using BuildingBlocks.Shared.Abstractions;
using MeetingCore.Entities;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AgendaTemplateVO;
using MeetingInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeetingInfrastructure.Repositories
{
    public class AgendaTemplateRepository : IAgendaTemplateRepository
    {
        private readonly MeetingDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public AgendaTemplateRepository(MeetingDbContext context)
        {
            _context = context;
            _unitOfWork = new UnitOfWork(context);
        }

        public IUnitOfWork UnitOfWork => _unitOfWork;

        public async Task AddAsync(AgendaTemplate template, CancellationToken ct = default)
        {
            await _context.AgendaTemplates.AddAsync(template, ct);
        }

        public async Task<AgendaTemplate?> GetByIdAsync(AgendaTemplateId id, CancellationToken ct = default)
        {
            // نحتاج البنود عند التعديل أو النسخ
            return await _context.AgendaTemplates
                .Include(t => t.Items)
                .AsSplitQuery()
                .FirstOrDefaultAsync(t => t.Id == id, ct);
        }

        public async Task<List<AgendaTemplate>> GetAllAsync(CancellationToken ct = default)
        {
            // لا نجلب البنود هنا لتحسين الأداء في القوائم
            return await _context.AgendaTemplates
                .AsNoTracking()
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(ct);
        }

        public void Delete(AgendaTemplate template)
        {
            _context.AgendaTemplates.Remove(template);
        }
    }
}
