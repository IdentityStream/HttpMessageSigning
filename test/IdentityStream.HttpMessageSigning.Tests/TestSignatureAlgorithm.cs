using System.Security.Cryptography;
using System.Text;

namespace IdentityStream.HttpMessageSigning.Tests {

    public class TestSignatureAlgorithm : ISignatureAlgorithm {
        public TestSignatureAlgorithm(HashAlgorithmName hashAlgorithm) {
            HashAlgorithm = hashAlgorithm;
        }

        public string Name => "TEST";

        public HashAlgorithmName HashAlgorithm { get; }

        public byte[] ComputeHash(string value) => Encoding.UTF8.GetBytes($"signed({value})");
    }
}
