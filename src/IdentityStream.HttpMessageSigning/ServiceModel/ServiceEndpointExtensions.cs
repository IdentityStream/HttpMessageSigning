using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;

namespace IdentityStream.HttpMessageSigning.ServiceModel {
    /// <summary>
    /// Extensions for hooking up HTTP message signing to a WCF service endpoint.
    /// </summary>
    public static class ServiceEndpointExtensions {
        private const string DeprecationMessage =
            "For parity with " + nameof(HttpClient) + " usage, " +
            "this overload has been deprecated and will be removed in a future version. " +
            "Construct an " + nameof(HttpMessageSigningConfiguration) + " instance and use the " + 
            nameof(UseHttpMessageSigning) + "(" + nameof(HttpMessageSigningConfiguration) + ") overload instead.";

        /// <summary>
        /// Adds HTTP message signing behavior using the specified <paramref name="certificate"/>
        /// to the specified <paramref name="endpoint"/>.
        /// </summary>
        /// <param name="endpoint">The endpoint to add the behavior to.</param>
        /// <param name="certificate">The certificate to use for signing HTTP messages.</param>
        [Obsolete(DeprecationMessage)]
        public static void UseHttpMessageSigning(this ServiceEndpoint endpoint, X509Certificate2 certificate) =>
            endpoint.UseHttpMessageSigning(certificate, configure: null);

        /// <summary>
        /// Adds HTTP message signing behavior using the specified <paramref name="certificate"/>
        /// to the specified <paramref name="endpoint"/>.
        /// </summary>
        /// <param name="endpoint">The endpoint to add the behavior to.</param>
        /// <param name="certificate">The certificate to use for signing HTTP messages.</param>
        /// <param name="configure">A delegate for changing the configuration before it's validated.</param>
        [Obsolete(DeprecationMessage)]
        public static void UseHttpMessageSigning(this ServiceEndpoint endpoint, X509Certificate2 certificate, Action<HttpMessageSigningConfiguration>? configure) =>
            endpoint.UseHttpMessageSigning(certificate, SignatureAlgorithm.DefaultHashAlgorithm, configure);

        /// <summary>
        /// Adds HTTP message signing behavior using the specified <paramref name="certificate"/>
        /// and <paramref name="hashAlgorithm"/> to the specified <paramref name="endpoint"/>.
        /// </summary>
        /// <param name="endpoint">The endpoint to add the behavior to.</param>
        /// <param name="certificate">The certificate to use for signing HTTP messages.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use for signing HTTP messages.</param>
        [Obsolete(DeprecationMessage)]
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
        [Obsolete(DeprecationMessage)]
        public static void UseHttpMessageSigning(this ServiceEndpoint endpoint, X509Certificate2 certificate, HashAlgorithmName hashAlgorithm, Action<HttpMessageSigningConfiguration>? configure) =>
            endpoint.UseHttpMessageSigning(certificate.GetKeyId(), certificate.GetSignatureAlgorithm(hashAlgorithm), configure);

        /// <summary>
        /// Adds HTTP message signing behavior using the specified <paramref name="keyId"/>
        /// and <paramref name="signatureAlgorithm"/> to the specified <paramref name="endpoint"/>.
        /// </summary>
        /// <param name="endpoint">The endpoint to add the behavior to.</param>
        /// <param name="keyId">The key ID to use for signing HTTP messages.</param>
        /// <param name="signatureAlgorithm">The signature algorithm to use for signing HTTP messages.</param>
        [Obsolete(DeprecationMessage)]
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
        [Obsolete(DeprecationMessage)]
        public static void UseHttpMessageSigning(this ServiceEndpoint endpoint, string keyId, ISignatureAlgorithm signatureAlgorithm, Action<HttpMessageSigningConfiguration>? configure) {
            endpoint.UseHttpMessageSigning(HttpMessageSigningConfiguration.Create(keyId, signatureAlgorithm, configure));
        }

        /// <summary>
        /// Adds HTTP message signing behavior using the specified <paramref name="config"/>.
        /// </summary>
        /// <param name="endpoint">The endpoint to add the behavior to.</param>
        /// <param name="config">The configuration to use for signing HTTP messages.</param>
        public static void UseHttpMessageSigning(this ServiceEndpoint endpoint, HttpMessageSigningConfiguration config) {
            if (!endpoint.EndpointBehaviors.Contains(typeof(HttpMessageSigningEndpointBehavior))) {
                endpoint.EndpointBehaviors.Add(new HttpMessageSigningEndpointBehavior(config));
            }
        }
    }
}
