using MediatR;

namespace CommitteeApplication.Features.StatusHistories.Commands.Models
{
    public class AddCommitStatusHistoryCommand : IRequest<Guid>
    {
        public Guid CommitteeId { get; set; }
        public int OldStatusId { get; set; }
        public int NewStatusId { get; set; }
        public string DecisionText { get; set; } = string.Empty;
        public string DecisionDocumentUrl { get; set; } = string.Empty;
    }
}
