using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SparebankenVest.HttpMessageSigning {
    internal static class HttpContentExtensions {
        public static async Task<string> GetDigestHeaderValues(this HttpContent content, HashAlgorithmName digestAlgorithm) {
            if (content is null) {
                throw new ArgumentNullException(nameof(content));
            }

            var bytes = await content.ReadAsByteArrayAsync().ConfigureAwait(false);

            using var hashAlgorithm = HashAlgorithm.Create(digestAlgorithm.Name!);
            if (hashAlgorithm is null) {
                throw new InvalidOperationException($"Invalid digest algorithm: {digestAlgorithm.Name}");
            }

            var digestBytes = hashAlgorithm.ComputeHash(bytes);
            var digest = Convert.ToBase64String(digestBytes);
            var algorithmName = GetDigestAlgorithmName(digestAlgorithm);

            return $"{algorithmName}={digest}";
        }

        private static string GetDigestAlgorithmName(HashAlgorithmName name) =>
            name.Name switch {
                "SHA256" => "SHA-256",
                "SHA384" => "SHA-384",
                "SHA512" => "SHA-512",
                "SHA1" => "SHA-1",
                _ => name.Name!,
            };
    }
}
