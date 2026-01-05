namespace BuildingBlocks.Shared.Exceptions
{
    public class IdentityServiceUnavailableException : Exception
    {
        public IdentityServiceUnavailableException(string message) : base(message) { }
        public IdentityServiceUnavailableException(string message, Exception inner) : base(message, inner) { }
    }
}
