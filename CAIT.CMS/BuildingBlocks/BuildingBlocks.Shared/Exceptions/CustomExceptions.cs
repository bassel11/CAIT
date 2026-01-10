namespace BuildingBlocks.Shared.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string name, object key) : base($"Entity '{name}' ({key}) was not found.") { }
    }

    public class BadRequestException : Exception
    {
        public string? Details { get; }
        public BadRequestException(string message) : base(message) { }
        public BadRequestException(string message, string details) : base(message) { Details = details; }
    }

    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }
        public ValidationException(IDictionary<string, string[]> errors) : base("One or more validation errors occurred.")
        {
            Errors = errors;
        }
    }

    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }

    public class InternalServerException : Exception
    {
        public string? Details { get; }
        public InternalServerException(string message) : base(message) { }
        public InternalServerException(string message, string details) : base(message) { Details = details; }
    }

    public class IdentityServiceUnavailableException : Exception
    {
        public IdentityServiceUnavailableException(string message) : base(message) { }
        public IdentityServiceUnavailableException(string message, Exception inner) : base(message, inner) { }

    }
}
