namespace Identity.Application.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// يرسل كود MFA لمستخدم عبر البريد الإلكتروني
        /// </summary>
        /// <param name="email">البريد الإلكتروني للمستلم</param>
        /// <param name="code">كود MFA المؤقت</param>
        Task SendMfaCodeAsync(string email, string code);

        /// <summary>
        /// إرسال رسالة بريد عامة (اختياري)
        /// </summary>
        Task SendEmailAsync(string email, string subject, string body);
    }
}
