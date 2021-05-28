using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using System.Text;

namespace IdentityStream.HttpMessageSigning.ServiceModel {
    public static class ServiceEndpointExtensions {
        public static ServiceEndpoint UseHttpMessageSigning(this ServiceEndpoint endpoint, X509Certificate2 certificate) =>
            endpoint.UseHttpMessageSigning(certificate, configure: null);

        public static ServiceEndpoint UseHttpMessageSigning(this ServiceEndpoint endpoint, X509Certificate2 certificate, Action<HttpMessageSigningConfiguration>? configure) =>
            endpoint.UseHttpMessageSigning(certificate, SignatureAlgorithm.DefaultHashAlgorithm, configure);

        public static ServiceEndpoint UseHttpMessageSigning(this ServiceEndpoint endpoint, X509Certificate2 certificate, HashAlgorithmName hashAlgorithm) =>
            endpoint.UseHttpMessageSigning(certificate, hashAlgorithm, configure: null);

        public static ServiceEndpoint UseHttpMessageSigning(this ServiceEndpoint endpoint, X509Certificate2 certificate, HashAlgorithmName hashAlgorithm, Action<HttpMessageSigningConfiguration>? configure) =>
            endpoint.UseHttpMessageSigning(GenerateKeyId(certificate.GetCertHash()), certificate.GetSignatureAlgorithm(hashAlgorithm), configure);

        public static ServiceEndpoint UseHttpMessageSigning(this ServiceEndpoint endpoint, string keyId, ISignatureAlgorithm signatureAlgorithm) =>
            endpoint.UseHttpMessageSigning(keyId, signatureAlgorithm, configure: null);

        public static ServiceEndpoint UseHttpMessageSigning(this ServiceEndpoint endpoint, string keyId, ISignatureAlgorithm signatureAlgorithm, Action<HttpMessageSigningConfiguration>? configure) {
            if (endpoint.EndpointBehaviors.Contains(typeof(HttpMessageSigningEndpointBehavior))) {
                return endpoint;
            }

            var config = HttpMessageSigningConfiguration.Create(keyId, signatureAlgorithm, configure);

            endpoint.EndpointBehaviors.Add(new HttpMessageSigningEndpointBehavior(config));

            return endpoint;
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
