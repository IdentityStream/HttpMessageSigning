using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace IdentityStream.HttpMessageSigning {
    internal static class HttpMessageExtensions {
        public static async Task<string> GetDigestHeaderValues(this IHttpMessage message, HashAlgorithmName digestAlgorithm) {
            if (message.Content is null) {
                return string.Empty;
            }

            var bytes = await message.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

            using var hashAlgorithm = HashAlgorithm.Create(digestAlgorithm.ToString());
            var digestBytes = hashAlgorithm.ComputeHash(bytes);
            var digest = Convert.ToBase64String(digestBytes);
            var algorightmName = GetDigestAlgorithmName(digestAlgorithm);

            return $"{algorightmName}={digest}";
        }

        public static bool HasHeader(this IHttpMessage message, string name) => message.TryGetHeaderValues(name, out _);

        private static string GetDigestAlgorithmName(HashAlgorithmName name) =>
            name.Name switch {
                "SHA256" => "SHA-256",
                "SHA384" => "SHA-384",
                "SHA512" => "SHA-512",
                "SHA1" => "SHA-1",
                _ => name.Name,
            };
    }
}
