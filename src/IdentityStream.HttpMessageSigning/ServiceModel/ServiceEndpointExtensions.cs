using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using System.Text;

namespace IdentityStream.HttpMessageSigning.ServiceModel {
    /// <summary>
    /// Extensions for hooking up HTTP message signing to a WCF service endpoint.
    /// </summary>
    public static class ServiceEndpointExtensions {
        /// <summary>
        /// Adds HTTP message signing behavior using the specified <paramref name="certificate"/>
        /// to the specified <paramref name="endpoint"/>.
        /// </summary>
        /// <param name="endpoint">The endpoint to add the behavior to.</param>
        /// <param name="certificate">The certificate to use for signing HTTP messages.</param>
        public static void UseHttpMessageSigning(this ServiceEndpoint endpoint, X509Certificate2 certificate) =>
            endpoint.UseHttpMessageSigning(certificate, configure: null);

        /// <summary>
        /// Adds HTTP message signing behavior using the specified <paramref name="certificate"/>
        /// to the specified <paramref name="endpoint"/>.
        /// </summary>
        /// <param name="endpoint">The endpoint to add the behavior to.</param>
        /// <param name="certificate">The certificate to use for signing HTTP messages.</param>
        /// <param name="configure">A delegate for changing the configuration before it's validated.</param>
        public static void UseHttpMessageSigning(this ServiceEndpoint endpoint, X509Certificate2 certificate, Action<HttpMessageSigningConfiguration>? configure) =>
            endpoint.UseHttpMessageSigning(certificate, SignatureAlgorithm.DefaultHashAlgorithm, configure);

        /// <summary>
        /// Adds HTTP message signing behavior using the specified <paramref name="certificate"/>
        /// and <paramref name="hashAlgorithm"/> to the specified <paramref name="endpoint"/>.
        /// </summary>
        /// <param name="endpoint">The endpoint to add the behavior to.</param>
        /// <param name="certificate">The certificate to use for signing HTTP messages.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use for signing HTTP messages.</param>
        public static void UseHttpMessageSigning(this ServiceEndpoint endpoint, X509Certificate2 certificate, HashAlgorithmName hashAlgorithm) =>
            endpoint.UseHttpMessageSigning(certificate, hashAlgorithm, configure: null);

        /// <summary>
        /// Adds HTTP message signing behavior using the specified <paramref name="certificate"/>
        /// and <paramref name="hashAlgorithm"/> to the specified <paramref name="endpoint"/>.
        /// </summary>
        /// <param name="endpoint">The endpoint to add the behavior to.</param>
        /// <param name="certificate">The certificate to use for signing HTTP messages.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use for signing HTTP messages.</param>
        /// <param name="configure">A delegate for changing the configuration before it's validated.</param>
        public static void UseHttpMessageSigning(this ServiceEndpoint endpoint, X509Certificate2 certificate, HashAlgorithmName hashAlgorithm, Action<HttpMessageSigningConfiguration>? configure) =>
            endpoint.UseHttpMessageSigning(GenerateKeyId(certificate.GetCertHash()), certificate.GetSignatureAlgorithm(hashAlgorithm), configure);

        /// <summary>
        /// Adds HTTP message signing behavior using the specified <paramref name="keyId"/>
        /// and <paramref name="signatureAlgorithm"/> to the specified <paramref name="endpoint"/>.
        /// </summary>
        /// <param name="endpoint">The endpoint to add the behavior to.</param>
        /// <param name="keyId">The key ID to use for signing HTTP messages.</param>
        /// <param name="signatureAlgorithm">The signature algorithm to use for signing HTTP messages.</param>
        public static void UseHttpMessageSigning(this ServiceEndpoint endpoint, string keyId, ISignatureAlgorithm signatureAlgorithm) =>
            endpoint.UseHttpMessageSigning(keyId, signatureAlgorithm, configure: null);

        /// <summary>
        /// Adds HTTP message signing behavior using the specified <paramref name="keyId"/>
        /// and <paramref name="signatureAlgorithm"/> to the specified <paramref name="endpoint"/>.
        /// </summary>
        /// <param name="endpoint">The endpoint to add the behavior to.</param>
        /// <param name="keyId">The key ID to use for signing HTTP messages.</param>
        /// <param name="signatureAlgorithm">The signature algorithm to use for signing HTTP messages.</param>
        /// <param name="configure">A delegate for changing the configuration before it's validated.</param>
        public static void UseHttpMessageSigning(this ServiceEndpoint endpoint, string keyId, ISignatureAlgorithm signatureAlgorithm, Action<HttpMessageSigningConfiguration>? configure) {
            if (endpoint.EndpointBehaviors.Contains(typeof(HttpMessageSigningEndpointBehavior))) {
                return;
            }

            var config = HttpMessageSigningConfiguration.Create(keyId, signatureAlgorithm, configure);

            endpoint.EndpointBehaviors.Add(new HttpMessageSigningEndpointBehavior(config));
        }

        private static string GenerateKeyId(byte[] hash) {
            var builder = new StringBuilder();
            const int length = 6;

            foreach (var @byte in hash.Skip(hash.Length - length)) {
                builder.Append(@byte.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
