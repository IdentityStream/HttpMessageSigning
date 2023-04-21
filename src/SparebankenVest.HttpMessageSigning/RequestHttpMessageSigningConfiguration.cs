using System;
using System.Collections.Immutable;
using System.Security.Cryptography;

namespace SparebankenVest.HttpMessageSigning {
    /// <summary>
    /// An immutable per-request signing configuration.
    /// </summary>
    internal class RequestHttpMessageSigningConfiguration {
        public RequestHttpMessageSigningConfiguration(HttpMessageSigningConfiguration config, IHttpMessage message) {
            KeyId = config.KeyId;
            SignatureAlgorithm = config.SignatureAlgorithm;
            DigestAlgorithm = config.DigestAlgorithm;
            GetCurrentTimestamp = config.GetCurrentTimestamp;
            HeadersToInclude = GetHeadersToInclude(config, message);
            Expires = config.Expires;
            HeaderValues = config.HeaderValues.ToImmutableDictionary();
            RequestTargetUriFormat = config.RequestTargetUriFormat;
        }

        public string KeyId { get; }

        public ISignatureAlgorithm SignatureAlgorithm { get; }

        public UriFormat RequestTargetUriFormat { get; }

        public HashAlgorithmName? DigestAlgorithm { get; }

        public Func<DateTimeOffset> GetCurrentTimestamp { get; }

        public IImmutableSet<string> HeadersToInclude { get; }

        public TimeSpan? Expires { get; }

        public IImmutableDictionary<string, Func<IHttpMessage, string>> HeaderValues { get; }

        private static IImmutableSet<string> GetHeadersToInclude(HttpMessageSigningConfiguration config, IHttpMessage message) {
            var headersToInclude = ImmutableSortedSet.CreateBuilder<string>(StringComparer.OrdinalIgnoreCase);

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

            if (message.Content is null) {
                headersToInclude.Remove(HeaderNames.Digest);
            } else if (config.DigestAlgorithm.HasValue) {
                headersToInclude.Add(HeaderNames.Digest);
            }

            return headersToInclude.ToImmutable();
        }
    }
}
