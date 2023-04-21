using System;
using System.Security.Cryptography;
using Hasher = System.Security.Cryptography.HashAlgorithm;

namespace SparebankenVest.HttpMessageSigning {
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
            using var hasher = Hasher.Create(HashAlgorithm.Name!);
            if (hasher is null) {
                throw new InvalidOperationException($"Invalid hash algorithm: {HashAlgorithm.Name}");
            }
            var hashedBytes = hasher.ComputeHash(bytes);
            return Rsa.SignHash(hashedBytes, HashAlgorithm, RSASignaturePadding.Pkcs1);
        }
    }
}