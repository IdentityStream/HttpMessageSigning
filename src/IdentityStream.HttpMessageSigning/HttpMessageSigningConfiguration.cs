using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace IdentityStream.HttpMessageSigning {
    /// <summary>
    /// Configuration for signing HTTP messages.
    /// </summary>
    public class HttpMessageSigningConfiguration {
        private HttpMessageSigningConfiguration(string keyId, ISignatureAlgorithm signatureAlgorithm) {
            KeyId = keyId;
            SignatureAlgorithm = signatureAlgorithm;
            GetCurrentTimestamp = () => DateTimeOffset.UtcNow;
            HeadersToInclude = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            HeaderValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            AddRecommendedHeaders = true;
        }

        /// <summary>
        /// Gets the opaque string that the server can use to look
        /// up the component they need to validate the signature.
        /// </summary>
        public string KeyId { get; }

        /// <summary>
        /// Gets or sets algorithm used to construct the signature string.
        /// </summary>
        public ISignatureAlgorithm SignatureAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the hash algorithm to produce a <c>Digest</c>
        /// header based on the HTTP message's body.
        /// </summary>
        public HashAlgorithmName? DigestAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the function to produce a timestamp. Useful for testing purposes.
        /// </summary>
        public Func<DateTimeOffset> GetCurrentTimestamp { get; set; }

        /// <summary>
        /// Gets or sets whether recommended headers should be automatically added to the signature.
        /// </summary>
        public bool AddRecommendedHeaders { get; set; }

        /// <summary>
        /// Gets the headers to include in the signature signing string.
        /// </summary>
        public ISet<string> HeadersToInclude { get; }

        /// <summary>
        /// Gets or sets the expiry time of a signature.
        /// </summary>
        public TimeSpan? Expires { get; set; }

        internal Dictionary<string, string> HeaderValues { get; }

        /// <summary>
        /// Adds a header value to all signed requests and includes it in the signature.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value to include.</param>
        public void AddHeaderValue(string name, string value)
        {
            HeadersToInclude.Add(name);
            HeaderValues[name] = value;
        }

        /// <summary>
        /// Adds a collection of headers to all signed requests and includes it in the signature.
        /// </summary>
        /// <param name="headers">The headers.</param>
        public void AddHeaderValues(IEnumerable<KeyValuePair<string, string>> headers) {
            foreach (var header in headers) {
                AddHeaderValue(header.Key, header.Value);
            }
        }

        /// <summary>
        /// Creates a configuration with the specified <paramref name="keyId"/> and <paramref name="signatureAlgorithm"/>.
        /// </summary>
        /// <param name="keyId">The key ID to use.</param>
        /// <param name="signatureAlgorithm">The signature algorithm to use.</param>
        /// <returns>A new configuration with the specified values.</returns>
        public static HttpMessageSigningConfiguration Create(string keyId, ISignatureAlgorithm signatureAlgorithm) {
            return Create(keyId, signatureAlgorithm, configure: null);
        }

        /// <summary>
        /// Creates a configuration with the specified <paramref name="keyId"/> and <paramref name="signatureAlgorithm"/>,
        /// with an optional <paramref name="configure"/> delegate for further configuration..
        /// </summary>
        /// <param name="keyId">The key ID to use.</param>
        /// <param name="signatureAlgorithm">The signature algorithm to use.</param>
        /// <param name="configure">A delegate for changing the configuration before it's validated.</param>
        /// <returns>A new configuration with the specified values.</returns>
        public static HttpMessageSigningConfiguration Create(string keyId, ISignatureAlgorithm signatureAlgorithm, Action<HttpMessageSigningConfiguration>? configure) {
            var config = new HttpMessageSigningConfiguration(keyId, signatureAlgorithm);

            configure?.Invoke(config);

            Validate(config);

            return config;
        }

        private static void Validate(HttpMessageSigningConfiguration config) {
            if (string.IsNullOrEmpty(config.KeyId)) {
                throw new InvalidOperationException($"{nameof(KeyId)} is required.");
            }

            if (config.SignatureAlgorithm is null) {
                throw new InvalidOperationException($"{nameof(SignatureAlgorithm)} is required.");
            }

            if (config.GetCurrentTimestamp is null) {
                throw new InvalidOperationException($"{nameof(GetCurrentTimestamp)} is required.");
            }

            if (config.HeadersToInclude.Contains(HeaderNames.Digest) && !config.DigestAlgorithm.HasValue) {
                throw new InvalidOperationException($"{nameof(DigestAlgorithm)} must be set when the {HeaderNames.Digest} header is included.");
            }

            if (config.HeadersToInclude.Contains(HeaderNames.Expires) && !config.Expires.HasValue) {
                throw new InvalidOperationException($"{nameof(Expires)} must be set when the {HeaderNames.Expires} header is included.");
            }
        }
    }
}
