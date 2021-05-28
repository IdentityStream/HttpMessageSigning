using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace IdentityStream.HttpMessageSigning {
    /// <summary>
    /// Extensions to get a signature algorithm based on a X509 certificate.
    /// </summary>
    public static class X509Certificate2Extensions {
        /// <summary>
        /// Gets a signature algorithm based on the cryptography of the provided <paramref name="certificate"/>,
        /// using the default <see cref="HashAlgorithm"/> (SHA512).
        /// </summary>
        /// <param name="certificate">The certificate to get a signing algorithm for.</param>
        /// <returns>signature algorithm based on the cryptography of the provided <paramref name="certificate"/>.</returns>
        public static ISignatureAlgorithm GetSignatureAlgorithm(this X509Certificate2 certificate) =>
            certificate.GetSignatureAlgorithm(SignatureAlgorithm.DefaultHashAlgorithm);

        /// <summary>
        /// Gets a signature algorithm based on the cryptography of the provided <paramref name="certificate"/>,
        /// using the specified <paramref name="hashAlgorithm"/>.
        /// </summary>
        /// <param name="certificate">The certificate to get a signing algorithm for.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use.</param>
        /// <returns>signature algorithm based on the cryptography of the provided <paramref name="certificate"/>.</returns>
        public static ISignatureAlgorithm GetSignatureAlgorithm(this X509Certificate2 certificate, HashAlgorithmName hashAlgorithm) {
            if (certificate.HasPrivateKey) {
                var rsa = certificate.GetRSAPrivateKey();
                if (rsa != null) {
                    return SignatureAlgorithm.Create(rsa, hashAlgorithm);
                }

                var ecdsa = certificate.GetECDsaPrivateKey();
                if (ecdsa != null) {
                    return SignatureAlgorithm.Create(ecdsa, hashAlgorithm);
                }
            }

            throw new NotSupportedException($"Unable to get private key from certificate: {certificate}");
        }
    }
}
