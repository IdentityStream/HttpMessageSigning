using System.Security.Cryptography;

namespace IdentityStream.HttpMessageSigning {
    public static class SignatureAlgorithm {
        internal static readonly HashAlgorithmName DefaultHashAlgorithm = HashAlgorithmName.SHA512;

        public static ISignatureAlgorithm Create(RSA rsa) =>
            Create(rsa, DefaultHashAlgorithm);

        public static ISignatureAlgorithm Create(RSA rsa, HashAlgorithmName hashAlgorithm) =>
            new RSASignatureAlgorithm(rsa, hashAlgorithm);

        public static ISignatureAlgorithm Create(ECDsa ecdsa) =>
            Create(ecdsa, DefaultHashAlgorithm);

        public static ISignatureAlgorithm Create(ECDsa ecdsa, HashAlgorithmName hashAlgorithm) =>
            new ECDsaSignatureAlgorithm(ecdsa, hashAlgorithm);
    }
}
