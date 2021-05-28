using System;

namespace IdentityStream.HttpMessageSigning {
    internal static class SignatureAlgorithmExtensions {
        public static bool ShouldIncludeDateHeader(this ISignatureAlgorithm algorithm) =>
            algorithm.Name.StartsWith("rsa", StringComparison.OrdinalIgnoreCase)
                || algorithm.Name.StartsWith("hmac", StringComparison.OrdinalIgnoreCase)
                || algorithm.Name.StartsWith("ecdsa", StringComparison.OrdinalIgnoreCase);

        public static bool ShouldIncludeCreatedHeader(this ISignatureAlgorithm algorithm) =>
            !algorithm.Name.StartsWith("rsa", StringComparison.OrdinalIgnoreCase)
                && !algorithm.Name.StartsWith("hmac", StringComparison.OrdinalIgnoreCase)
                && !algorithm.Name.StartsWith("ecdsa", StringComparison.OrdinalIgnoreCase);
    }
}
