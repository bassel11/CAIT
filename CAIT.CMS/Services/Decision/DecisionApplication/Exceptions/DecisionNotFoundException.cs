using BuildingBlocks.Shared.Exceptions;

namespace DecisionApplication.Exceptions
{
    public class DecisionNotFoundException : NotFoundException
    {
        public DecisionNotFoundException(Guid id) : base("Decision", id)
        {
        }
    }
}
