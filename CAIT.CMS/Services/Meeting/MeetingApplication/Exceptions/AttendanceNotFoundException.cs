namespace MeetingApplication.Exceptions
{
    public class AttendanceNotFoundException : ApplicationException
    {
        public AttendanceNotFoundException(string name, Object key) : base($"Entity {name} - {key} is not found.")
        {

        }
    }
}
