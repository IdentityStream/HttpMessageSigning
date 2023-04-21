using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SparebankenVest.HttpMessageSigning {
    /// <summary>
    /// Configuration for signing HTTP messages.
    /// </summary>
    public class HttpMessageSigningConfiguration {
        /// <summary>
        /// Creates an instance of <see cref="HttpMessageSigningConfiguration"/> with the specified <paramref name="keyId"/> and <paramref name="signatureAlgorithm"/>;
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="signatureAlgorithm"></param>
        public HttpMessageSigningConfiguration(string keyId, ISignatureAlgorithm signatureAlgorithm) {
            KeyId = keyId ?? throw new ArgumentNullException(nameof(keyId));
            SignatureAlgorithm = signatureAlgorithm ?? throw new ArgumentNullException(nameof(signatureAlgorithm));
            GetCurrentTimestamp = () => DateTimeOffset.UtcNow;
            HeadersToInclude = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            HeaderValues = new Dictionary<string, Func<IHttpMessage, string>>(StringComparer.OrdinalIgnoreCase);
            RequestTargetUriFormat = UriFormat.Unescaped;
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
        public ISignatureAlgorithm SignatureAlgorithm { get; }

        /// <summary>
        /// Gets or sets the URI format used when constructing the <c>(request-target)</c> header.
        /// </summary>
        public UriFormat RequestTargetUriFormat { get; set; }

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

        internal Dictionary<string, Func<IHttpMessage, string>> HeaderValues { get; }

        /// <summary>
        /// Adds a header value to all signed requests and includes it in the signature.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value to include.</param>
        public void AddHeaderValue(string name, string value) => AddHeaderValue(name, _ => value);

        /// <summary>
        /// Adds a header value using the provided <paramref name="getter"/> to all signed requests and includes it in the signature.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <param name="getter">The header value getter to include.</param>
        public void AddHeaderValue(string name, Func<IHttpMessage, string> getter) {
            HeadersToInclude.Add(name);
            HeaderValues[name] = getter;
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
        /// Creates a configuration based on the specified <paramref name="certificate"/>.
        /// </summary>
        /// <param name="certificate">The certificate to use for signing HTTP messages.</param>
        /// <returns>A new configuration based on the specified <paramref name="certificate"/>.</returns>
        public static HttpMessageSigningConfiguration FromCertificate(X509Certificate2 certificate) =>
            FromCertificate(certificate, HttpMessageSigning.SignatureAlgorithm.DefaultHashAlgorithm);

        /// <summary>
        /// Creates a configuration based on the specified <paramref name="certificate"/> and <paramref name="hashAlgorithm"/>.
        /// </summary>
        /// <param name="certificate">The certificate to use for signing HTTP messages.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use for signing HTTP messages.</param>
        /// <returns>A new configuration based on the specified <paramref name="certificate"/> and <paramref name="hashAlgorithm"/>.</returns>
        public static HttpMessageSigningConfiguration FromCertificate(X509Certificate2 certificate, HashAlgorithmName hashAlgorithm) =>
            new(certificate.GetKeyId(), certificate.GetSignatureAlgorithm(hashAlgorithm));

        /// <summary>
        /// Validates the configuration.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if configuration is invalid.</exception>
        public HttpMessageSigningConfiguration Validate() {
            if (GetCurrentTimestamp is null) {
                throw new InvalidOperationException($"{nameof(GetCurrentTimestamp)} is required.");
            }

            if (HeadersToInclude.Contains(HeaderNames.Digest) && !DigestAlgorithm.HasValue) {
                throw new InvalidOperationException($"{nameof(DigestAlgorithm)} must be set when the {HeaderNames.Digest} header is included.");
            }

            if (HeadersToInclude.Contains(HeaderNames.Expires) && !Expires.HasValue) {
                throw new InvalidOperationException($"{nameof(Expires)} must be set when the {HeaderNames.Expires} header is included.");
            }

            return this;
        }
    }
}
