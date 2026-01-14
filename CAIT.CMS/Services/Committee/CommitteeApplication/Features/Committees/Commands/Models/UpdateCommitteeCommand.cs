using CommitteeCore.Entities;
using MediatR;

namespace CommitteeApplication.Features.Committees.Commands.Models
{
    public class UpdateCommitteeCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Purpose { get; set; }
        public string Scope { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public CommitteeType Type { get; set; }
        public decimal? Budget { get; set; }
        public string CreationDecisionText { get; set; } = string.Empty;
        public string? CreationDecisionDocumentUrl { get; set; }
        public string UpdatedDecisionText { get; set; } = string.Empty;

        //// يمكن استخدام Count فقط لتقليل حجم الاستجابة
        //public int MembersCount { get; set; }
        //public int DocumentsCount { get; set; }
        //public int DecisionsCount { get; set; }
    }
}
