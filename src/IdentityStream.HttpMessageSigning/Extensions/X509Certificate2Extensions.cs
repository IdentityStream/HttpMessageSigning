using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace IdentityStream.HttpMessageSigning {
    public static class X509Certificate2Extensions {
        public static ISignatureAlgorithm GetSignatureAlgorithm(this X509Certificate2 certificate) =>
            certificate.GetSignatureAlgorithm(SignatureAlgorithm.DefaultHashAlgorithm);

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
