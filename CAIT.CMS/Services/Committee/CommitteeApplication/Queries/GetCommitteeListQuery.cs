using CommitteeApplication.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitteeApplication.Queries
{
    public class GetCommitteeListQuery : IRequest<List<CommitteeResponse>>
    {
        public Guid Id { get; set; }
        public GetCommitteeListQuery(Guid id)
        {
            Id = id;
        }
    }
}
