using System;
using System.Security.Cryptography;

namespace IdentityStream.HttpMessageSigning {
    // ReSharper disable once InconsistentNaming
    internal class RSASignatureAlgorithm : ISignatureAlgorithm {
        public RSASignatureAlgorithm(RSA rsa, HashAlgorithmName hashAlgorithm) {
            Rsa = rsa ?? throw new ArgumentNullException(nameof(rsa));
            HashAlgorithm = hashAlgorithm;
        }

        public RSA Rsa { get; }

        public HashAlgorithmName HashAlgorithm { get; }

        public string Name => "RSA";

        public byte[] ComputeHash(byte[] bytes) {
            using var hasher = Hasher.GetSha(HashAlgorithm);
            var hashedBytes = hasher.ComputeHash(bytes);
            return Rsa.SignHash(hashedBytes, HashAlgorithm, RSASignaturePadding.Pkcs1);
        }
    }
}