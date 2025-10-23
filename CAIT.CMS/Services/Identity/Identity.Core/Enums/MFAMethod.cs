namespace Identity.Core.Enums
{
    public enum MFAMethod
    {

        None = 0,    // Default - no MFA
        Email = 1,   // Email code verification
        TOTP = 2     // Authenticator app (Google/Microsoft Authenticator)

    }
}
