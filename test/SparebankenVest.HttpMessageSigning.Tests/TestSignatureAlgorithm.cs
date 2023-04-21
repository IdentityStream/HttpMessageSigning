using System.Security.Cryptography;
using System.Text;

namespace SparebankenVest.HttpMessageSigning.Tests {

    public class TestSignatureAlgorithm : ISignatureAlgorithm {
        public TestSignatureAlgorithm(HashAlgorithmName hashAlgorithm) {
            HashAlgorithm = hashAlgorithm;
        }

        public string Name => "TEST";

        public HashAlgorithmName HashAlgorithm { get; }

        public byte[] ComputeHash(byte[] bytes) => Encoding.UTF8.GetBytes($"signed({Encoding.UTF8.GetString(bytes)})");
    }
}
