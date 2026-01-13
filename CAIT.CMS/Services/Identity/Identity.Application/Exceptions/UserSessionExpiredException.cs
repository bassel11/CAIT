namespace Identity.Application.Exceptions
{
    public class UserSessionExpiredException : Exception
    {
        public UserSessionExpiredException(string message) : base(message) { }
    }
}
