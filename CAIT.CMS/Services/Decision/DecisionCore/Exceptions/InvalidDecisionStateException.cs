namespace DecisionCore.Exceptions
{
    public class InvalidDecisionStateException : DomainException
    {
        public InvalidDecisionStateException(Guid decisionId)
            : base($"Decision with id '{decisionId}' cannot be modified in its current state.")
        {
        }
    }
}
