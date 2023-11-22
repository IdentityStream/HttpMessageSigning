using System;
using System.Security.Cryptography;

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
            using var hasher = Hasher.GetSha(HashAlgorithm);
            var hashedBytes = hasher.ComputeHash(bytes);
            return Ecdsa.SignHash(hashedBytes);
        }
    }
}