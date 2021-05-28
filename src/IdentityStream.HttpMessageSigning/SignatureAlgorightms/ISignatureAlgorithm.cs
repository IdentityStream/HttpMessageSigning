using System.Security.Cryptography;

namespace IdentityStream.HttpMessageSigning {
    /// <summary>
    /// Represents an algorithm to use for signing HTTP messages.
    /// </summary>
    public interface ISignatureAlgorithm {
        /// <summary>
        /// Gets the name of the algorithm.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the hash algorithm to use.
        /// </summary>
        HashAlgorithmName HashAlgorithm { get; }

        /// <summary>
        /// Computes a hash based on the specified input <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to hash.</param>
        /// <returns>Hashed bytes from the specified value.</returns>
        byte[] ComputeHash(string value);
    }
}
