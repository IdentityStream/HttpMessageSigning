using System;
using System.Security.Cryptography;
using Hasher = System.Security.Cryptography.HashAlgorithm;

namespace IdentityStream.HttpMessageSigning {
    // ReSharper disable once InconsistentNaming
    internal class ECDsaSignatureAlgorithm : ISignatureAlgorithm {
        public ECDsaSignatureAlgorithm(ECDsa ecdsa, HashAlgorithmName hashAlgorithm) {
            Ecdsa = ecdsa ?? throw new ArgumentNullException(nameof(ecdsa));
            HashAlgorithm = hashAlgorithm;
        }

        public ECDsa Ecdsa { get; }

        public HashAlgorithmName HashAlgorithm { get; }

        public string Name => "ECDsa";

        public byte[] ComputeHash(byte[] bytes) {
            using var hasher = Hasher.Create(HashAlgorithm.Name!)
                ?? throw new InvalidOperationException($"Invalid hash algorithm: {HashAlgorithm.Name}");
            var hashedBytes = hasher.ComputeHash(bytes);
            return Ecdsa.SignHash(hashedBytes);
        }
    }
}