using System;
using System.Threading.Tasks;

namespace IdentityStream.HttpMessageSigning {
    /// <summary>
    /// Class to use for signing HTTP requests.
    /// </summary>
    public static class HttpMessageSigner {
        /// <summary>
        /// Signs the provided <paramref name="message"/> according to the provided <paramref name="config"/>.
        /// </summary>
        /// <param name="message">The HTTP message to sign.</param>
        /// <param name="config">The configuration to use when signing.</param>
        public static async Task SignAsync(IHttpMessage message, HttpMessageSigningConfiguration config) {
            var timestamp = config.GetCurrentTimestamp();

            await AddRequiredHeaders(message, config, timestamp).ConfigureAwait(false);

            AddSignatureHeader(message, config, timestamp);
        }

        private static async Task AddRequiredHeaders(IHttpMessage message, HttpMessageSigningConfiguration config, DateTimeOffset timestamp) {
            if (ShouldInclude(HeaderNames.Date)) {
                message.SetHeader(HeaderNames.Date, timestamp.ToString("R"));
            }

            if (ShouldInclude(HeaderNames.Host))
            {
                message.SetHeader(HeaderNames.Host,
                    message.RequestUri.GetComponents(
                        UriComponents.NormalizedHost | // Always convert punycode to Unicode.
                        UriComponents.Host, UriFormat.Unescaped));
            }

            if (ShouldInclude(HeaderNames.Digest) && config.DigestAlgorithm.HasValue) {
                var digestHeaderValue = await message.GetDigestHeaderValues(config.DigestAlgorithm.Value).ConfigureAwait(false);
                message.SetHeader(HeaderNames.Digest, digestHeaderValue);
            }

            bool ShouldInclude(string name) {
                return config.HeadersToInclude.Contains(name) && !message.HasHeader(name);
            }
        }

        private static void AddSignatureHeader(IHttpMessage message, HttpMessageSigningConfiguration config, DateTimeOffset timestamp) {
            var signingString = SigningStringComposer.Compose(message, config, timestamp);

            var signatureHash = config.SignatureAlgorithm.ComputeHash(signingString);
            var signatureString = Convert.ToBase64String(signatureHash);

            var signatureHeaderValue = SignatureHeaderComposer.Compose(signatureString, config, timestamp);
            message.SetHeader(HeaderNames.Signature, signatureHeaderValue);
        }
    }
}
