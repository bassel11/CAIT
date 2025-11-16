using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitteeApplication.Features.Committees.Commands.Models
{
    public class DeleteCommitteeCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
