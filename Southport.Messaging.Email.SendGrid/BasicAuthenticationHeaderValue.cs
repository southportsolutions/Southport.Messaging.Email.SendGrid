using System.Net.Http.Headers;
using System.Text;

// ReSharper disable once CheckNamespace
namespace System.Net.Http
{
    /// <summary>
    /// HTTP Basic Authentication authorization header
    /// </summary>
    /// <seealso cref="System.Net.Http.Headers.AuthenticationHeaderValue" />
    public class BasicAuthenticationHeaderValue : AuthenticationHeaderValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthenticationHeaderValue"/> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public BasicAuthenticationHeaderValue(string userName, string password)
            : base("Basic", EncodeCredential(userName, password))
        { }

        /// <summary>
        /// Encodes the credential.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">userName</exception>
        public static string EncodeCredential(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            password ??= "";

            var encoding = Encoding.UTF8;
            var credential = $"{userName}:{password}";

            return Convert.ToBase64String(encoding.GetBytes(credential));
        }
    }
}
