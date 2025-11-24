namespace MeetingApplication.Exceptions
{
    public class AgendaNotFoundException : ApplicationException
    {
        public AgendaNotFoundException(string name, Object key) : base($"Entity {name} - {key} is not found.")
        {

        }
    }
}
