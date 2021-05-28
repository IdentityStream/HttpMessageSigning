using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace IdentityStream.HttpMessageSigning {
    public class HttpMessageSigningConfiguration {
        private HttpMessageSigningConfiguration(string keyId, ISignatureAlgorithm signatureAlgorithm) {
            KeyId = keyId;
            SignatureAlgorithm = signatureAlgorithm;
            GetUtcNow = () => DateTimeOffset.UtcNow;
            HeadersToInclude = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            AddRecommendedHeaders = true;
        }

        public string KeyId { get; }

        public ISignatureAlgorithm SignatureAlgorithm { get; set; }

        public HashAlgorithmName? DigestAlgorithm { get; set; }

        public Func<DateTimeOffset> GetUtcNow { get; set; }

        public bool AddRecommendedHeaders { get; set; }

        public ISet<string> HeadersToInclude { get; }

        public TimeSpan? Expires { get; set; }

        public static HttpMessageSigningConfiguration Create(string keyId, ISignatureAlgorithm signatureAlgorithm) {
            return Create(keyId, signatureAlgorithm, configure: null);
        }

        public static HttpMessageSigningConfiguration Create(string keyId, ISignatureAlgorithm signatureAlgorithm, Action<HttpMessageSigningConfiguration>? configure) {
            var config = new HttpMessageSigningConfiguration(keyId, signatureAlgorithm);

            configure?.Invoke(config);

            AdjustHeadersToInclude(config);

            return config;
        }

        private static void AdjustHeadersToInclude(HttpMessageSigningConfiguration config) {
            // Chek if use has opted out of adding recommended headers.
            if (!config.AddRecommendedHeaders) {
                return;
            }

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
