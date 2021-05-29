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

            if (config.AddRecommendedHeaders) {
                EnsureRecommendedHeaders(config);
            }

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
        }

        private static void EnsureRecommendedHeaders(HttpMessageSigningConfiguration config) {
            // According to the spec, the (request-target) header should always be part of the signature string.
            config.HeadersToInclude.Add(HeaderNames.RequestTarget);

            // According to the spec, the Date header SHOULD be included when the algorithm starts with 'rsa', 'hmac' or 'ecdsa'.
            if (config.SignatureAlgorithm.ShouldIncludeDateHeader()) {
                config.HeadersToInclude.Add(HeaderNames.Date);
            }

            // According to the spec, the (created) header SHOULD be included when the algorithm does not start with 'rsa', 'hmac' or 'ecdsa'.
            if (config.SignatureAlgorithm.ShouldIncludeCreatedHeader()) {
                config.HeadersToInclude.Add(HeaderNames.Created);
            }

            // Include the (expires) header in the signature string if it's been enabled.
            if (config.Expires.HasValue) {
                config.HeadersToInclude.Add(HeaderNames.Expires);
            }

            // Include the Digest header in the signature string if it's been enabled.
            if (config.DigestAlgorithm.HasValue) {
                config.HeadersToInclude.Add(HeaderNames.Digest);
            }
        }
    }
}
