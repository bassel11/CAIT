namespace MeetingCore.Enums
{
    public enum QuorumType
    {
        Percentage = 1,        // e.g., 66.67%
        PercentagePlusOne = 2, // e.g., 50% +1
        AbsoluteNumber = 3    // e.g., at least 7 members
    }
}
