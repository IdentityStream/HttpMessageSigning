using System;
using System.Text;
using System.Threading.Tasks;

namespace SparebankenVest.HttpMessageSigning {
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
            var requestConfig = new RequestHttpMessageSigningConfiguration(config, message);

            var timestamp = requestConfig.GetCurrentTimestamp();

            await AddRequiredHeaders(message, requestConfig, timestamp).ConfigureAwait(false);

            AddSignatureHeader(message, requestConfig, timestamp);
        }

        private static async Task AddRequiredHeaders(IHttpMessage message, RequestHttpMessageSigningConfiguration config, DateTimeOffset timestamp) {
            if (ShouldInclude(HeaderNames.Date)) {
                message.SetHeader(HeaderNames.Date, timestamp.ToString("R"));
            }

            if (ShouldInclude(HeaderNames.Host))
            {
                var hostHeaderValue = message.RequestUri.GetComponents(
                    UriComponents.NormalizedHost | // Always convert punycode to Unicode.
                    UriComponents.Host, UriFormat.Unescaped);
                message.SetHeader(HeaderNames.Host, hostHeaderValue);
            }

            if (ShouldInclude(HeaderNames.Digest) && config.DigestAlgorithm.HasValue && message.Content != null) {
                var digestHeaderValue = await message.Content
                    .GetDigestHeaderValues(config.DigestAlgorithm.Value)
                    .ConfigureAwait(false);
                message.SetHeader(HeaderNames.Digest, digestHeaderValue);
            }

            foreach (var header in config.HeaderValues)
            {
                if (ShouldInclude(header.Key)) {
                    message.SetHeader(header.Key, header.Value(message));
                }
            }

            bool ShouldInclude(string name) {
                return config.HeadersToInclude.Contains(name) && !message.HasHeader(name);
            }
        }

        private static void AddSignatureHeader(IHttpMessage message, RequestHttpMessageSigningConfiguration config, DateTimeOffset timestamp) {
            var signingString = SigningStringComposer.Compose(message, config, timestamp);

            // Add the signing string to the request so it can be inspected later :)
            message.SetProperty(Constants.SigningString, signingString);

            var plainBytes = Encoding.UTF8.GetBytes(signingString);
            var signatureHash = config.SignatureAlgorithm.ComputeHash(plainBytes);
            var signatureString = Convert.ToBase64String(signatureHash);

            var signatureHeaderValue = SignatureHeaderComposer.Compose(signatureString, config, timestamp);
            message.SetHeader(HeaderNames.Signature, signatureHeaderValue);
        }
    }
}
