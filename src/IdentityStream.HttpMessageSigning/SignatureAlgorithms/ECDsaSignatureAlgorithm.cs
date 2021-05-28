using System;
using System.Security.Cryptography;
using System.Text;
using Hasher = System.Security.Cryptography.HashAlgorithm;

namespace IdentityStream.HttpMessageSigning {
    internal class ECDsaSignatureAlgorithm : ISignatureAlgorithm {
        public ECDsaSignatureAlgorithm(ECDsa? ecdsa, HashAlgorithmName hashAlgorithm) {
            Ecdsa = ecdsa ?? throw new ArgumentNullException(nameof(ecdsa));
            HashAlgorithm = hashAlgorithm;
        }

        public ECDsa Ecdsa { get; }

        public HashAlgorithmName HashAlgorithm { get; }

        public string Name => "ECDsa";

        public byte[] ComputeHash(string value) {
            using var hasher = Hasher.Create(HashAlgorithm.ToString());
            var plainBytes = Encoding.UTF8.GetBytes(value);
            var hashedBytes = hasher.ComputeHash(plainBytes);
            return Ecdsa.SignHash(hashedBytes);
        }
    }
}