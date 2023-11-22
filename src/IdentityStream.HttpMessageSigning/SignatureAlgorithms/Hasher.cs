using System;
using System.Security.Cryptography;

namespace IdentityStream.HttpMessageSigning
{
    internal static class Hasher
    {
        public static HashAlgorithm GetSha(HashAlgorithmName name) => name.Name switch
        {
            nameof(HashAlgorithmName.SHA256) => SHA256.Create(),
            nameof(HashAlgorithmName.SHA384) => SHA384.Create(),
            nameof(HashAlgorithmName.SHA512) => SHA512.Create(),
            _ => throw new NotSupportedException($"The specified hash algorithm '{name.Name}' is not supported."),
        };

        public static HMAC GetHmac(HashAlgorithmName name, byte[] key) => name.Name switch
        {
            nameof(HashAlgorithmName.SHA256) => new HMACSHA256(key),
            nameof(HashAlgorithmName.SHA384) => new HMACSHA384(key),
            nameof(HashAlgorithmName.SHA512) => new HMACSHA512(key),
            _ => throw new NotSupportedException($"The specified hash algorithm '{name.Name}' is not supported."),
        };
    }
}