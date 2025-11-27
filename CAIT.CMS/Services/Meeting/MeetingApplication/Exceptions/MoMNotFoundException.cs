namespace MeetingApplication.Exceptions
{
    public class MoMNotFoundException : ApplicationException
    {
        public MoMNotFoundException(string name, Object key) : base($"Entity {name} - {key} is not found.")
        {

        }
    }
}
