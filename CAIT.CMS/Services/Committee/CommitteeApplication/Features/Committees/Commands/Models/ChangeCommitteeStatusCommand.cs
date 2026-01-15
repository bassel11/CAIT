using MediatR;

namespace CommitteeApplication.Features.Committees.Commands.Models
{
    public class ChangeCommitteeStatusCommand : IRequest<Unit>
    {
        public Guid CommitteeId { get; set; }

        // الحالة الجديدة المطلوبة
        public int NewStatusId { get; set; }

        // [متطلب أساسي] نص القرار الذي بني عليه التغيير (إلزامي للتوثيق)
        public string DecisionText { get; set; } = string.Empty;

        // رابط وثيقة القرار (اختياري)
        public string? DecisionDocumentUrl { get; set; }
    }
}
