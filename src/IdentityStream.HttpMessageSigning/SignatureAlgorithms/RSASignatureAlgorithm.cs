using System;
using System.Security.Cryptography;
using System.Text;
using Hasher = System.Security.Cryptography.HashAlgorithm;

namespace IdentityStream.HttpMessageSigning {
    // ReSharper disable once InconsistentNaming
    internal class RSASignatureAlgorithm : ISignatureAlgorithm {
        public RSASignatureAlgorithm(RSA? rsa, HashAlgorithmName hashAlgorithm) {
            Rsa = rsa ?? throw new ArgumentNullException(nameof(rsa));
            HashAlgorithm = hashAlgorithm;
        }

        public RSA Rsa { get; }

        public HashAlgorithmName HashAlgorithm { get; }

        public string Name => "RSA";

        public byte[] ComputeHash(string value) {
            using var hasher = Hasher.Create(HashAlgorithm.Name!);
            if (hasher is null) {
                throw new InvalidOperationException($"Invalid hash algorithm: {HashAlgorithm.Name}");
            }
            var plainBytes = Encoding.UTF8.GetBytes(value);
            var hashedBytes = hasher.ComputeHash(plainBytes);
            return Rsa.SignHash(hashedBytes, HashAlgorithm, RSASignaturePadding.Pkcs1);
        }
    }
}