using MeetingApplication.Integrations;
using System.Text;

namespace MeetingInfrastructure.Pdf
{
    public class SimpleHtmlPdfGenerator : IPdfGenerator
    {
        public byte[] GeneratePdfFromHtml(string html)
        {
            // Placeholder: replace with real generator (Wkhtmltopdf, DinkToPdf, or external service)
            var fake = Encoding.UTF8.GetBytes("PDF(" + html + ")");
            return fake;
        }
    }
}
