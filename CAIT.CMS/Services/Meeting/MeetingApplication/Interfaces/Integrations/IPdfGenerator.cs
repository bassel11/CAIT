namespace MeetingApplication.Interfaces.Integrations
{
    public interface IPdfGenerator
    {
        byte[] GeneratePdfFromHtml(string html);
    }
}
