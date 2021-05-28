using System.Security.Cryptography;

namespace IdentityStream.HttpMessageSigning {
    public interface ISignatureAlgorithm {
        string Name { get; }

        HashAlgorithmName HashAlgorithm { get; }

        byte[] ComputeHash(string value);
    }
}
