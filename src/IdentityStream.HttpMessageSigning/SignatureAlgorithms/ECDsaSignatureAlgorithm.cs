using System;
using System.Security.Cryptography;
using System.Text;
using Hasher = System.Security.Cryptography.HashAlgorithm;

namespace IdentityStream.HttpMessageSigning {
    // ReSharper disable once InconsistentNaming
    internal class ECDsaSignatureAlgorithm : ISignatureAlgorithm {
        public ECDsaSignatureAlgorithm(ECDsa? ecdsa, HashAlgorithmName hashAlgorithm) {
            Ecdsa = ecdsa ?? throw new ArgumentNullException(nameof(ecdsa));
            HashAlgorithm = hashAlgorithm;
        }

        public ECDsa Ecdsa { get; }

        public HashAlgorithmName HashAlgorithm { get; }

        public string Name => "ECDsa";

        public byte[] ComputeHash(string value) {
            using var hasher = Hasher.Create(HashAlgorithm.Name!);
            if (hasher is null) {
                throw new InvalidOperationException($"Invalid hash algorithm: {HashAlgorithm.Name}");
            }
            var plainBytes = Encoding.UTF8.GetBytes(value);
            var hashedBytes = hasher.ComputeHash(plainBytes);
            return Ecdsa.SignHash(hashedBytes);
        }
    }
}