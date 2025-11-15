using MediatR;

namespace CommitteeApplication.Queries
{
    public class GetComMembersListQuery : IRequest<List<CommitteeMemberResponse>>
    {
    }
}
