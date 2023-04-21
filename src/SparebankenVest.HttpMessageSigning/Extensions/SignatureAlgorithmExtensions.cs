using System;

namespace SparebankenVest.HttpMessageSigning {
    internal static class SignatureAlgorithmExtensions {
        public static bool ShouldIncludeDateHeader(this ISignatureAlgorithm algorithm) =>
            algorithm.Name.StartsWith("rsa", StringComparison.OrdinalIgnoreCase)
                || algorithm.Name.StartsWith("hmac", StringComparison.OrdinalIgnoreCase)
                || algorithm.Name.StartsWith("ecdsa", StringComparison.OrdinalIgnoreCase);

        public static bool ShouldIncludeCreatedHeader(this ISignatureAlgorithm algorithm) =>
            !algorithm.Name.StartsWith("rsa", StringComparison.OrdinalIgnoreCase)
                && !algorithm.Name.StartsWith("hmac", StringComparison.OrdinalIgnoreCase)
                && !algorithm.Name.StartsWith("ecdsa", StringComparison.OrdinalIgnoreCase);

        public static string GetAlgorithmName(this ISignatureAlgorithm algorithm) {
            var signatureAlgorithmName = algorithm.Name.ToLowerInvariant();
            var hashAlgorithmName = algorithm.HashAlgorithm.Name!.ToLowerInvariant();
            return $"{signatureAlgorithmName}-{hashAlgorithmName}";
        }
    }
}
