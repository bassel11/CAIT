namespace CommitteeApplication.Exceptions
{
    public class CommitteeMemberNotFoundException : ApplicationException
    {
        public CommitteeMemberNotFoundException(string name, Object key) : base($"Entity {name} - {key} is not found.")
        {

        }
    }
}
