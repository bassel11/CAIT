using CommitteeCore.Entities;

namespace CommitteeApplication.Features.Committees.Queries.Results
{
    public class GetCommitteeListResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Purpose { get; set; }
        public string Scope { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CommitteeType Type { get; set; }
        public CommitteeStatus Status { get; set; }
        public decimal? Budget { get; set; }
        public string CreationDecisionText { get; set; }
        public string UpdatedDecisionText { get; set; }

        // يمكن استخدام Count فقط لتقليل حجم الاستجابة
        public int MembersCount { get; set; }
        public int DocumentsCount { get; set; }
        public int DecisionsCount { get; set; }
    }
}
