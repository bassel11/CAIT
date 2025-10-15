using Identity.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Novell.Directory.Ldap;

namespace Identity.Infrastructure.Services
{
    public class LdapAuthService : ILdapAuthService
    {
        private readonly IConfiguration _config;

        public LdapAuthService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<(bool Success, string? ExternalId, string? Error)> AuthenticateAsync(string username, string password)
        {
            var ldapConfig = _config.GetSection("Ldap");
            string url = ldapConfig["Url"]!;
            string bindDn = ldapConfig["BindDn"]!;
            string bindPassword = ldapConfig["BindPassword"]!;
            string baseDn = ldapConfig["BaseDn"]!;
            string userFilter = ldapConfig["UserFilter"] ?? "(sAMAccountName={0})";

            try
            {
                var uri = new Uri(url);

                using var conn = new LdapConnection
                {
                    SecureSocketLayer = uri.Scheme.Equals("ldaps", StringComparison.OrdinalIgnoreCase)
                };

                await conn.ConnectAsync(uri.Host, uri.Port);
                await conn.BindAsync(bindDn, bindPassword);

                var filter = string.Format(userFilter, username);

                var searchResults = await conn.SearchAsync(
                    baseDn,
                    LdapConnection.ScopeSub,
                    filter,
                    new[] { "dn", "sAMAccountName", "objectGUID" },
                    false
                );

                // ✅ In v4.0.0, SearchAsync returns IAsyncEnumerable<LdapEntry>
                LdapEntry? entry = null;
                await foreach (var result in searchResults)
                {
                    entry = result;
                    break; // نأخذ أول نتيجة فقط
                }
                if (entry == null)
                {
                    return (false, null, "User not found in LDAP/AD");
                }

                var userDn = entry.Dn;

                try
                {
                    using var userConn = new LdapConnection
                    {
                        SecureSocketLayer = uri.Scheme.Equals("ldaps", StringComparison.OrdinalIgnoreCase)
                    };

                    await userConn.ConnectAsync(uri.Host, uri.Port);
                    await userConn.BindAsync(userDn, password);

                    // ✅ Attributes are accessed from entry.Attributes (IDictionary<string, LdapAttribute>)
                    var attributes = entry.GetAttributeSet();

                    string? externalId = null;
                    if (attributes.ContainsKey("objectGUID"))
                    {
                        var guidAttr = attributes["objectGUID"];
                        externalId = BitConverter.ToString(guidAttr.ByteValue).Replace("-", "");
                    }
                    else if (attributes.ContainsKey("sAMAccountName"))
                    {
                        externalId = attributes["sAMAccountName"].StringValue;
                    }
                    else
                    {
                        externalId = username;
                    }

                    return (true, externalId, null);
                }
                catch (LdapException)
                {
                    return (false, null, "Invalid credentials");
                }
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }
    }
}
