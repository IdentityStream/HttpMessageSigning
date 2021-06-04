using System;
using System.Collections.Immutable;
using System.Security.Cryptography;

namespace IdentityStream.HttpMessageSigning {
    /// <summary>
    /// An immutable per-request signing configuration.
    /// </summary>
    internal class RequestHttpMessageSigningConfiguration {
        private RequestHttpMessageSigningConfiguration(
            string keyId,
            ISignatureAlgorithm signatureAlgorithm,
            HashAlgorithmName? digestAlgorithm,
            Func<DateTimeOffset> getCurrentTimestamp,
            IImmutableSet<string> headersToInclude,
            TimeSpan? expires,
            IImmutableDictionary<string, string> headerValues) {
            KeyId = keyId;
            SignatureAlgorithm = signatureAlgorithm;
            DigestAlgorithm = digestAlgorithm;
            GetCurrentTimestamp = getCurrentTimestamp;
            HeadersToInclude = headersToInclude;
            Expires = expires;
            HeaderValues = headerValues;
        }

        public string KeyId { get; }

        public ISignatureAlgorithm SignatureAlgorithm { get; }

        public HashAlgorithmName? DigestAlgorithm { get; }

        public Func<DateTimeOffset> GetCurrentTimestamp { get; }

        public IImmutableSet<string> HeadersToInclude { get; }

        public TimeSpan? Expires { get; }

        public IImmutableDictionary<string, string> HeaderValues { get; }

        public static RequestHttpMessageSigningConfiguration Create(HttpMessageSigningConfiguration config, IHttpMessage message) {
            return new RequestHttpMessageSigningConfiguration(
                config.KeyId,
                config.SignatureAlgorithm,
                config.DigestAlgorithm,
                config.GetCurrentTimestamp,
                GetRequestHeadersToInclude(config, message),
                config.Expires,
                config.HeaderValues.ToImmutableDictionary());
        }

        private static IImmutableSet<string> GetRequestHeadersToInclude(HttpMessageSigningConfiguration config, IHttpMessage message) {
            var headersToInclude = ImmutableSortedSet.CreateBuilder(StringComparer.OrdinalIgnoreCase);

            foreach (var header in config.HeadersToInclude) {
                headersToInclude.Add(header);
            }

            if (config.AddRecommendedHeaders) {
                // According to the spec, the (request-target) header should always be part of the signature string.
                headersToInclude.Add(HeaderNames.RequestTarget);

                // According to the spec, the Date header SHOULD be included when the algorithm starts with 'rsa', 'hmac' or 'ecdsa'.
                if (config.SignatureAlgorithm.ShouldIncludeDateHeader()) {
                    headersToInclude.Add(HeaderNames.Date);
                }

                // According to the spec, the (created) header SHOULD be included when the algorithm does not start with 'rsa', 'hmac' or 'ecdsa'.
                if (config.SignatureAlgorithm.ShouldIncludeCreatedHeader()) {
                    headersToInclude.Add(HeaderNames.Created);
                }
            }

            if (config.Expires.HasValue) {
                headersToInclude.Add(HeaderNames.Expires);
            }

            if (message.Content == null) {
                headersToInclude.Remove(HeaderNames.Digest);
            } else if (config.DigestAlgorithm.HasValue) {
                headersToInclude.Add(HeaderNames.Digest);
            }

            return headersToInclude.ToImmutable();
        }
    }
}
