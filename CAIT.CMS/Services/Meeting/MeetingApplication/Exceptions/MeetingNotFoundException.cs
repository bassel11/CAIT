namespace MeetingApplication.Exceptions
{
    public class MeetingNotFoundException : ApplicationException
    {
        public MeetingNotFoundException(string name, Object key) : base($"Entity {name} - {key} is not found.")
        {

        }
    }
}
